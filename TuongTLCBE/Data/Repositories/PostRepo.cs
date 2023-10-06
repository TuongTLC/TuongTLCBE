using EntityFrameworkPaginateCore;
using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class PostRepo : Repository<Post>
{
    private readonly PostCategoryRepo _postCategoryRepo;
    private readonly PostTagRepo _postTagRepo;
    private readonly CategoryRepo _categoryRepo;
    private readonly TagRepo _tagRepo;
    private readonly UserRepo _userRepo;

    public PostRepo(TuongTlcdbContext context, PostCategoryRepo postCategoryRepo, PostTagRepo postTagRepo,
        CategoryRepo categoryRepo, TagRepo tagRepo, UserRepo userRepo) : base(context)
    {
        _postCategoryRepo = postCategoryRepo;
        _postTagRepo = postTagRepo;
        _categoryRepo = categoryRepo;
        _tagRepo = tagRepo;
        _userRepo = userRepo;
    }

    public async Task<PostInfoModel?> GetPostInfo(Guid postId)
    {
        try
        {
            var post = await context.Posts.Where(x => x.Id.Equals(postId)).FirstOrDefaultAsync();
            if (post != null)
            {
                var author = await _userRepo.GetAuthor(post.Author);
                PostModel postModel = new()
                {
                    Id = post.Id,
                    PostName = post.PostName,
                    Summary = post.Summary,
                    Content = post.Content,
                    CreateDate = post.CreateDate,
                    Author = author,
                    Like = post.Like,
                    Dislike = post.Dislike,
                    Thumbnail = post.Thumbnail,
                    Status = post.Status
                };
                var postCategories = await _postCategoryRepo.GetPostCategories(postId);
                List<PostCategoryModel> postCategoryModels = new();
                if (postCategories.Any())
                    foreach (var cate in postCategories)
                    {
                        var category = await _categoryRepo.Get(cate.CategoryId);
                        if (category != null)
                        {
                            PostCategoryModel model = new()
                            {
                                Id = category.Id,
                                CategoryName = category.CategoryName,
                                Description = category.Description
                            };
                            postCategoryModels.Add(model);
                        }
                    }

                var postTags = await _postTagRepo.GetPostTags(postId);
                List<PostTagModel> postTagModels = new();
                if (postTags.Any())
                    foreach (var postTag in postTags)
                    {
                        var tag = await _tagRepo.Get(postTag.TagId);
                        if (tag != null)
                        {
                            PostTagModel model = new()
                            {
                                Id = tag.Id,
                                TagName = tag.TagName,
                                Description = tag.Description
                            };
                            postTagModels.Add(model);
                        }
                    }

                var postInfo = new PostInfoModel
                {
                    PostInfo = postModel,
                    PostCategories = postCategoryModels,
                    PostTags = postTagModels
                };
                return postInfo;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<object> GetPostsInfo(int pageNumber, int pageSize, string? status, Guid? categoryId, Guid? tagId)
    {
        try
        {            
            List<Post>? posts;
            PostPagingResponseModel responseModel = new();
            switch (status)
            {
                
                case "all":
                    posts = await context.Posts.OrderByDescending(x => x.CreateDate).ToListAsync();
                    if (posts.Any())
                    {
                        List<PostInfoModel> listPosts = new();
                        foreach (var post in posts)
                        {
                            var author = await _userRepo.GetAuthor(post.Author);
                            PostModel postModel = new()
                            {
                                Id = post.Id,
                                PostName = post.PostName,
                                Summary = post.Summary,
                                Content = post.Content,
                                CreateDate = post.CreateDate,
                                Author = author,
                                Like = post.Like,
                                Dislike = post.Dislike,
                                Thumbnail = post.Thumbnail,
                                Status = post.Status
                            };
                            var postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                            List<PostCategoryModel> postCategoryModels = new();
                            if (postCategories.Any())
                                foreach (var cate in postCategories)
                                {
                                    var category = await _categoryRepo.Get(cate.CategoryId);
                                    if (category != null)
                                    {
                                        PostCategoryModel postCategoryModel = new()
                                        {
                                            Id = category.Id,
                                            CategoryName = category.CategoryName,
                                            Description = category.Description
                                        };
                                        postCategoryModels.Add(postCategoryModel);
                                    }
                                }

                            var postTags = await _postTagRepo.GetPostTags(post.Id);
                            List<PostTagModel> postTagModels = new();
                            if (postTags.Any())
                                foreach (var postTag in postTags)
                                {
                                    var tag = await _tagRepo.Get(postTag.TagId);
                                    if (tag != null)
                                    {
                                        PostTagModel postTagModel = new()
                                        {
                                            Id = tag.Id,
                                            TagName = tag.TagName,
                                            Description = tag.Description
                                        };
                                        postTagModels.Add(postTagModel);
                                    }
                                }
                            
                            var postInfo = new PostInfoModel
                            {
                                PostInfo = postModel,
                                PostCategories = postCategoryModels,
                                PostTags = postTagModels
                            };
                            listPosts.Add(postInfo);
                        }

                        if (categoryId != null && categoryId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postCate in post.PostCategories)
                                {
                                    if (postCate.Id.Equals(categoryId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }
                            }
                            listPosts = newListPosts;
                        }

                        if (tagId != null &&tagId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postTag in post.PostTags)
                                {
                                    if (postTag.Id.Equals(tagId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }

                            }
                            listPosts = newListPosts;
                        }

                        var postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
                        
                        var paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                            .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                        responseModel.Paging = paging;
                        responseModel.ListPosts = postPaged.Results;
                    }
                    break;
                case "active":
                    posts = await context.Posts.Where(x=> x.Status.Equals(true)).OrderByDescending(x => x.CreateDate).ToListAsync();
                    if (posts.Any())
                    {
                        List<PostInfoModel> listPosts = new();
                        foreach (var post in posts)
                        {
                            var author = await _userRepo.GetAuthor(post.Author);
                            PostModel postModel = new()
                            {
                                Id = post.Id,
                                PostName = post.PostName,
                                Summary = post.Summary,
                                Content = post.Content,
                                CreateDate = post.CreateDate,
                                Author = author,
                                Like = post.Like,
                                Dislike = post.Dislike,
                                Thumbnail = post.Thumbnail,
                                Status = post.Status
                            };
                            var postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                            List<PostCategoryModel> postCategoryModels = new();
                            if (postCategories.Any())
                                foreach (var cate in postCategories)
                                {
                                    var category = await _categoryRepo.Get(cate.CategoryId);
                                    if (category != null)
                                    {
                                        PostCategoryModel postCategoryModel = new()
                                        {
                                            Id = category.Id,
                                            CategoryName = category.CategoryName,
                                            Description = category.Description
                                        };
                                        postCategoryModels.Add(postCategoryModel);
                                    }
                                }

                            var postTags = await _postTagRepo.GetPostTags(post.Id);
                            List<PostTagModel> postTagModels = new();
                            if (postTags.Any())
                                foreach (var postTag in postTags)
                                {
                                    var tag = await _tagRepo.Get(postTag.TagId);
                                    if (tag != null)
                                    {
                                        PostTagModel postTagModel = new()
                                        {
                                            Id = tag.Id,
                                            TagName = tag.TagName,
                                            Description = tag.Description
                                        };
                                        postTagModels.Add(postTagModel);
                                    }
                                }
                            
                            var postInfo = new PostInfoModel
                            {
                                PostInfo = postModel,
                                PostCategories = postCategoryModels,
                                PostTags = postTagModels
                            };
                            listPosts.Add(postInfo);
                        }

                        if (categoryId != null && categoryId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postCate in post.PostCategories)
                                {
                                    if (postCate.Id.Equals(categoryId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }
                            }
                            listPosts = newListPosts;
                        }

                        if (tagId != null &&tagId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postTag in post.PostTags)
                                {
                                    if (postTag.Id.Equals(tagId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }

                            }
                            listPosts = newListPosts;
                        }

                        var postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
                        
                        var paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                            .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                        responseModel.Paging = paging;
                        responseModel.ListPosts = postPaged.Results;
                    }
                    break;
                case "inactive":
                    posts = await context.Posts.Where(x=> x.Status.Equals(false)).OrderByDescending(x => x.CreateDate).ToListAsync();
                    if (posts.Any())
                    {
                        List<PostInfoModel> listPosts = new();
                        foreach (var post in posts)
                        {
                            var author = await _userRepo.GetAuthor(post.Author);
                            PostModel postModel = new()
                            {
                                Id = post.Id,
                                PostName = post.PostName,
                                Summary = post.Summary,
                                Content = post.Content,
                                CreateDate = post.CreateDate,
                                Author = author,
                                Like = post.Like,
                                Dislike = post.Dislike,
                                Thumbnail = post.Thumbnail,
                                Status = post.Status
                            };
                            var postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                            List<PostCategoryModel> postCategoryModels = new();
                            if (postCategories.Any())
                                foreach (var cate in postCategories)
                                {
                                    var category = await _categoryRepo.Get(cate.CategoryId);
                                    if (category != null)
                                    {
                                        PostCategoryModel postCategoryModel = new()
                                        {
                                            Id = category.Id,
                                            CategoryName = category.CategoryName,
                                            Description = category.Description
                                        };
                                        postCategoryModels.Add(postCategoryModel);
                                    }
                                }

                            var postTags = await _postTagRepo.GetPostTags(post.Id);
                            List<PostTagModel> postTagModels = new();
                            if (postTags.Any())
                                foreach (var postTag in postTags)
                                {
                                    var tag = await _tagRepo.Get(postTag.TagId);
                                    if (tag != null)
                                    {
                                        PostTagModel postTagModel = new()
                                        {
                                            Id = tag.Id,
                                            TagName = tag.TagName,
                                            Description = tag.Description
                                        };
                                        postTagModels.Add(postTagModel);
                                    }
                                }
                            
                            var postInfo = new PostInfoModel
                            {
                                PostInfo = postModel,
                                PostCategories = postCategoryModels,
                                PostTags = postTagModels
                            };
                            listPosts.Add(postInfo);
                        }

                        if (categoryId != null && categoryId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postCate in post.PostCategories)
                                {
                                    if (postCate.Id.Equals(categoryId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }
                            }
                            listPosts = newListPosts;
                        }

                        if (tagId != null &&tagId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postTag in post.PostTags)
                                {
                                    if (postTag.Id.Equals(tagId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }

                            }
                            listPosts = newListPosts;
                        }

                        var postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
                        
                        var paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                            .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                        responseModel.Paging = paging;
                        responseModel.ListPosts = postPaged.Results;
                    }
                    break;
            }
            return responseModel;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return e;
        }
    }
        public async Task<object> SearchPost(int pageNumber, int pageSize,string postName, string? status, Guid? categoryId, Guid? tagId)
    {
        try
        {            
            List<Post>? posts;
            PostPagingResponseModel responseModel = new();
            switch (status)
            {
                
                case "all":
                    posts = await context.Posts.Where(x=>x.PostName.Contains(postName)).OrderByDescending(x => x.CreateDate).ToListAsync();
                    if (posts.Any())
                    {
                        List<PostInfoModel> listPosts = new();
                        foreach (var post in posts)
                        {
                            var author = await _userRepo.GetAuthor(post.Author);
                            PostModel postModel = new()
                            {
                                Id = post.Id,
                                PostName = post.PostName,
                                Summary = post.Summary,
                                Content = post.Content,
                                CreateDate = post.CreateDate,
                                Author = author,
                                Like = post.Like,
                                Dislike = post.Dislike,
                                Thumbnail = post.Thumbnail,
                                Status = post.Status
                            };
                            var postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                            List<PostCategoryModel> postCategoryModels = new();
                            if (postCategories.Any())
                                foreach (var cate in postCategories)
                                {
                                    var category = await _categoryRepo.Get(cate.CategoryId);
                                    if (category != null)
                                    {
                                        PostCategoryModel postCategoryModel = new()
                                        {
                                            Id = category.Id,
                                            CategoryName = category.CategoryName,
                                            Description = category.Description
                                        };
                                        postCategoryModels.Add(postCategoryModel);
                                    }
                                }

                            var postTags = await _postTagRepo.GetPostTags(post.Id);
                            List<PostTagModel> postTagModels = new();
                            if (postTags.Any())
                                foreach (var postTag in postTags)
                                {
                                    var tag = await _tagRepo.Get(postTag.TagId);
                                    if (tag != null)
                                    {
                                        PostTagModel postTagModel = new()
                                        {
                                            Id = tag.Id,
                                            TagName = tag.TagName,
                                            Description = tag.Description
                                        };
                                        postTagModels.Add(postTagModel);
                                    }
                                }
                            
                            var postInfo = new PostInfoModel
                            {
                                PostInfo = postModel,
                                PostCategories = postCategoryModels,
                                PostTags = postTagModels
                            };
                            listPosts.Add(postInfo);
                        }

                        if (categoryId != null && categoryId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postCate in post.PostCategories)
                                {
                                    if (postCate.Id.Equals(categoryId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }
                            }
                            listPosts = newListPosts;
                        }

                        if (tagId != null &&tagId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postTag in post.PostTags)
                                {
                                    if (postTag.Id.Equals(tagId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }

                            }
                            listPosts = newListPosts;
                        }

                        var postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
                        
                        var paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                            .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                        responseModel.Paging = paging;
                        responseModel.ListPosts = postPaged.Results;
                    }
                    break;
                case "active":
                    posts = await context.Posts.Where(z=>z.PostName.Contains(postName)).Where(x=> x.Status.Equals(true)).OrderByDescending(x => x.CreateDate).ToListAsync();
                    if (posts.Any())
                    {
                        List<PostInfoModel> listPosts = new();
                        foreach (var post in posts)
                        {
                            var author = await _userRepo.GetAuthor(post.Author);
                            PostModel postModel = new()
                            {
                                Id = post.Id,
                                PostName = post.PostName,
                                Summary = post.Summary,
                                Content = post.Content,
                                CreateDate = post.CreateDate,
                                Author = author,
                                Like = post.Like,
                                Dislike = post.Dislike,
                                Thumbnail = post.Thumbnail,
                                Status = post.Status
                            };
                            var postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                            List<PostCategoryModel> postCategoryModels = new();
                            if (postCategories.Any())
                                foreach (var cate in postCategories)
                                {
                                    var category = await _categoryRepo.Get(cate.CategoryId);
                                    if (category != null)
                                    {
                                        PostCategoryModel postCategoryModel = new()
                                        {
                                            Id = category.Id,
                                            CategoryName = category.CategoryName,
                                            Description = category.Description
                                        };
                                        postCategoryModels.Add(postCategoryModel);
                                    }
                                }

                            var postTags = await _postTagRepo.GetPostTags(post.Id);
                            List<PostTagModel> postTagModels = new();
                            if (postTags.Any())
                                foreach (var postTag in postTags)
                                {
                                    var tag = await _tagRepo.Get(postTag.TagId);
                                    if (tag != null)
                                    {
                                        PostTagModel postTagModel = new()
                                        {
                                            Id = tag.Id,
                                            TagName = tag.TagName,
                                            Description = tag.Description
                                        };
                                        postTagModels.Add(postTagModel);
                                    }
                                }
                            
                            var postInfo = new PostInfoModel
                            {
                                PostInfo = postModel,
                                PostCategories = postCategoryModels,
                                PostTags = postTagModels
                            };
                            listPosts.Add(postInfo);
                        }

                        if (categoryId != null && categoryId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postCate in post.PostCategories)
                                {
                                    if (postCate.Id.Equals(categoryId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }
                            }
                            listPosts = newListPosts;
                        }

                        if (tagId != null &&tagId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postTag in post.PostTags)
                                {
                                    if (postTag.Id.Equals(tagId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }

                            }
                            listPosts = newListPosts;
                        }

                        var postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
                        
                        var paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                            .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                        responseModel.Paging = paging;
                        responseModel.ListPosts = postPaged.Results;
                    }
                    break;
                case "inactive":
                    posts = await context.Posts.Where(z=>z.PostName.Contains(postName)).Where(x=> x.Status.Equals(false)).OrderByDescending(x => x.CreateDate).ToListAsync();
                    if (posts.Any())
                    {
                        List<PostInfoModel> listPosts = new();
                        foreach (var post in posts)
                        {
                            var author = await _userRepo.GetAuthor(post.Author);
                            PostModel postModel = new()
                            {
                                Id = post.Id,
                                PostName = post.PostName,
                                Summary = post.Summary,
                                Content = post.Content,
                                CreateDate = post.CreateDate,
                                Author = author,
                                Like = post.Like,
                                Dislike = post.Dislike,
                                Thumbnail = post.Thumbnail,
                                Status = post.Status
                            };
                            var postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                            List<PostCategoryModel> postCategoryModels = new();
                            if (postCategories.Any())
                                foreach (var cate in postCategories)
                                {
                                    var category = await _categoryRepo.Get(cate.CategoryId);
                                    if (category != null)
                                    {
                                        PostCategoryModel postCategoryModel = new()
                                        {
                                            Id = category.Id,
                                            CategoryName = category.CategoryName,
                                            Description = category.Description
                                        };
                                        postCategoryModels.Add(postCategoryModel);
                                    }
                                }

                            var postTags = await _postTagRepo.GetPostTags(post.Id);
                            List<PostTagModel> postTagModels = new();
                            if (postTags.Any())
                                foreach (var postTag in postTags)
                                {
                                    var tag = await _tagRepo.Get(postTag.TagId);
                                    if (tag != null)
                                    {
                                        PostTagModel postTagModel = new()
                                        {
                                            Id = tag.Id,
                                            TagName = tag.TagName,
                                            Description = tag.Description
                                        };
                                        postTagModels.Add(postTagModel);
                                    }
                                }
                            
                            var postInfo = new PostInfoModel
                            {
                                PostInfo = postModel,
                                PostCategories = postCategoryModels,
                                PostTags = postTagModels
                            };
                            listPosts.Add(postInfo);
                        }

                        if (categoryId != null && categoryId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postCate in post.PostCategories)
                                {
                                    if (postCate.Id.Equals(categoryId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }
                            }
                            listPosts = newListPosts;
                        }

                        if (tagId != null &&tagId!=Guid.Empty)
                        {
                            List<PostInfoModel> newListPosts = new List<PostInfoModel>();
                            foreach (var post in listPosts)
                            {   
                                foreach (var postTag in post.PostTags)
                                {
                                    if (postTag.Id.Equals(tagId))
                                    {
                                        newListPosts.Add(post);
                                        break;
                                    }
                                }

                            }
                            listPosts = newListPosts;
                        }

                        var postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
                        
                        var paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                            .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                        responseModel.Paging = paging;
                        responseModel.ListPosts = postPaged.Results;
                    }
                    break;
            }
            return responseModel;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return e;
        }
    }

    
    public async Task<bool> ChangePostStatus(Guid postId, string status)
    {
        try
        {
            bool stat;
            if (status.Trim().ToLower().Equals("active"))
                stat = true;
            else if (status.Trim().ToLower().Equals("inactive"))
                stat = true;
            else
                return false;

            var post = await context.Posts.Where(x => x.Id.Equals(postId)).FirstOrDefaultAsync();
            if (post != null)
            {
                post.Status = stat;
                _ = await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdatePost(PostUpdateModel postUpdateModel)
    {
        try
        {
            var post = await context.Posts.Where(x => x.Id.Equals(postUpdateModel.Id)).FirstOrDefaultAsync();
            if (post != null)
            {
                post.PostName = postUpdateModel.PostName;
                post.Summary = postUpdateModel.Summary;
                post.Content = postUpdateModel.Content;
                post.Thumbnail = postUpdateModel.Thumbnail;
                _ = await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}