using EntityFrameworkPaginateCore;
using TuongTLCBE.Business.CacheService;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class PostService
{
    private readonly ICacheService _cacheService;
    private readonly CategoryRepo _categoryRepo;
    private readonly DecodeToken _decodeToken;
    private readonly EmailService _emailService;
    private readonly PostCategoryRepo _postCategoryRepo;
    private readonly PostRepo _postRepo;
    private readonly PostTagRepo _postTagRepo;
    private readonly TagRepo _tagRepo;
    private readonly UserRepo _userRepo;

    public PostService(DecodeToken decodeToken, PostCategoryRepo postCategoryRepo, PostTagRepo postTagRepo,
        CategoryRepo categoryRepo, TagRepo tagRepo, PostRepo postRepo, UserRepo userRepo, EmailService emailService,
        ICacheService cacheService)
    {
        _postCategoryRepo = postCategoryRepo;
        _postTagRepo = postTagRepo;
        _categoryRepo = categoryRepo;
        _tagRepo = tagRepo;
        _postRepo = postRepo;
        _decodeToken = decodeToken;
        _userRepo = userRepo;
        _emailService = emailService;
        _cacheService = cacheService;
    }

    public async Task<object> InsertPost(PostRequestModel postRequestModel, string token)
    {
        try
        {
            if (postRequestModel.PostName.Length < 6)
            {
                return "Post name too short!";
            }

            if (postRequestModel.Content.Length <= 50)
            {
                return "Content too short!";
            }

            if (!postRequestModel.CategoriesIds.Any() && !postRequestModel.TagsIds.Any())
            {
                return "Categories id or tag id is empty!";
            }

            foreach (Guid cateId in postRequestModel.CategoriesIds)
            {
                Category? categoryCheck = await _categoryRepo.Get(cateId);
                if (categoryCheck == null)
                {
                    return "Category " + cateId + " not found!";
                }
            }

            foreach (Guid tagId in postRequestModel.TagsIds)
            {
                Tag? tagCheck = await _tagRepo.Get(tagId);
                if (tagCheck == null)
                {
                    return "Tag " + tagId + " not found!";
                }
            }

            string userId = _decodeToken.Decode(token, "userid");
            Post post = new()
            {
                Id = Guid.NewGuid(),
                PostName = postRequestModel.PostName,
                Summary = postRequestModel.Summary,
                Content = postRequestModel.Content,
                CreateDate = DateTime.Now,
                Author = Guid.Parse(userId),
                Like = 0,
                Dislike = 0,
                Thumbnail = postRequestModel.Thumbnail,
                Status = false,
                AdminStatus = Enums.POST_REVIEW
            };
            Post? insertPost = await _postRepo.Insert(post);
            if (insertPost != null)
            {
                foreach (Guid cateInsert in postRequestModel.CategoriesIds)
                {
                    PostCategory postCategoryInsert = new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        CategoryId = cateInsert
                    };
                    _ = await _postCategoryRepo.Insert(postCategoryInsert);
                }

                foreach (Guid tagInsert in postRequestModel.TagsIds)
                {
                    PostTag postTagInsert = new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        TagId = tagInsert
                    };
                    _ = await _postTagRepo.Insert(postTagInsert);
                }

                object? postInfoModel = await GetPost(post.Id);
                if (postInfoModel != null)
                {
                    _ = await _emailService.NewPostNotification(post.Id);
                    _ = await _cacheService.RemoveOldCache("post");
                }

                return postInfoModel ?? "Something went wrong!!";
            }

            return "Insert post failed!";
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object?> GetPost(Guid postId)
    {
        try
        {
            Post? post = await _postRepo.Get(postId);
            if (post != null)
            {
                PostAuthor? author = await _userRepo.GetAuthor(post.Author);
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
                    Status = post.Status,
                    AdminStatus = post.AdminStatus
                };
                List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(postId);
                List<PostCategoryModel> postCategoryModels = new();
                if (postCategories.Any())
                {
                    foreach (PostCategory cate in postCategories)
                    {
                        Category? category = await _categoryRepo.Get(cate.CategoryId);
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
                }

                List<PostTag> postTags = await _postTagRepo.GetPostTags(postId);
                List<PostTagModel> postTagModels = new();
                if (postTags.Any())
                {
                    foreach (PostTag postTag in postTags)
                    {
                        Tag? tag = await _tagRepo.Get(postTag.TagId);
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
                }

                PostInfoModel postInfo = new()
                {
                    PostInfo = postModel,
                    PostCategories = postCategoryModels,
                    PostTags = postTagModels
                };
                return postInfo;
            }

            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> GetPosts(int pageNumber, int pageSize, string? status, string? adminStatus,
        Guid? categoryId,
        Guid? tagId)
    {
        try
        {
            status ??= "all";
            adminStatus ??= "all";
            PostPagingResponseModel responseModel = new();
            List<Post>? posts = await _postRepo.GetPostsInfo(status, adminStatus);
            List<PostInfoModel> listPosts = new();
            if (posts != null && posts.Any())
            {
                foreach (Post post in posts)
                {
                    PostAuthor? author = await _userRepo.GetAuthor(post.Author);
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
                        Status = post.Status,
                        AdminStatus = post.AdminStatus
                    };
                    List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                    List<PostCategoryModel> postCategoryModels = new();
                    if (postCategories.Any())
                    {
                        foreach (PostCategory cate in postCategories)
                        {
                            Category? category = await _categoryRepo.Get(cate.CategoryId);
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
                    }

                    List<PostTag> postTags = await _postTagRepo.GetPostTags(post.Id);
                    List<PostTagModel> postTagModels = new();
                    if (postTags.Any())
                    {
                        foreach (PostTag postTag in postTags)
                        {
                            Tag? tag = await _tagRepo.Get(postTag.TagId);
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
                    }

                    PostInfoModel postInfo = new()
                    {
                        PostInfo = postModel,
                        PostCategories = postCategoryModels,
                        PostTags = postTagModels
                    };
                    listPosts.Add(postInfo);
                }

                if (categoryId != null && categoryId != Guid.Empty)
                {
                    List<PostInfoModel> newListPosts = new();
                    foreach (PostInfoModel post in listPosts)
                    {
                        foreach (PostCategoryModel postCate in post.PostCategories)
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

                if (tagId != null && tagId != Guid.Empty)
                {
                    List<PostInfoModel> newListPosts = new();
                    foreach (PostInfoModel post in listPosts)
                    {
                        foreach (PostTagModel postTag in post.PostTags)
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
            }

            Page<PostInfoModel> postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
            PaginationResponseModel paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount == 0 ? 1 : postPaged.PageCount)
                .RecordCount(postPaged.RecordCount);
            responseModel.Paging = paging;
            responseModel.ListPosts = postPaged.Results;
            return responseModel;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> GetUserPostsByAdmin(int pageNumber, int pageSize, string userId)
    {
        try
        {
            PostPagingResponseModel responseModel = new();
            List<Post>? posts = await _postRepo.GetPostByUser(Guid.Parse(userId));
            if (posts != null && posts.Any())
            {
                List<PostInfoModel> listPosts = new();
                foreach (Post post in posts)
                {
                    PostAuthor? author = await _userRepo.GetAuthor(post.Author);
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
                        Status = post.Status,
                        AdminStatus = post.AdminStatus
                    };
                    List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                    List<PostCategoryModel> postCategoryModels = new();
                    if (postCategories.Any())
                    {
                        foreach (PostCategory cate in postCategories)
                        {
                            Category? category = await _categoryRepo.Get(cate.CategoryId);
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
                    }

                    List<PostTag> postTags = await _postTagRepo.GetPostTags(post.Id);
                    List<PostTagModel> postTagModels = new();
                    if (postTags.Any())
                    {
                        foreach (PostTag postTag in postTags)
                        {
                            Tag? tag = await _tagRepo.Get(postTag.TagId);
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
                    }

                    PostInfoModel postInfo = new()
                    {
                        PostInfo = postModel,
                        PostCategories = postCategoryModels,
                        PostTags = postTagModels
                    };
                    listPosts.Add(postInfo);
                }


                Page<PostInfoModel> postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);

                PaginationResponseModel paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                    .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                responseModel.Paging = paging;
                responseModel.ListPosts = postPaged.Results;
            }

            return responseModel;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> GetPostsByUser(int pageNumber, int pageSize, string token)
    {
        try
        {
            string userid = _decodeToken.Decode(token, "userid");
            PostPagingResponseModel responseModel = new();
            List<Post>? posts = await _postRepo.GetPostByUser(Guid.Parse(userid));
            if (posts != null && posts.Any())
            {
                List<PostInfoModel> listPosts = new();
                foreach (Post post in posts)
                {
                    PostAuthor? author = await _userRepo.GetAuthor(post.Author);
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
                        Status = post.Status,
                        AdminStatus = post.AdminStatus
                    };
                    List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                    List<PostCategoryModel> postCategoryModels = new();
                    if (postCategories.Any())
                    {
                        foreach (PostCategory cate in postCategories)
                        {
                            Category? category = await _categoryRepo.Get(cate.CategoryId);
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
                    }

                    List<PostTag> postTags = await _postTagRepo.GetPostTags(post.Id);
                    List<PostTagModel> postTagModels = new();
                    if (postTags.Any())
                    {
                        foreach (PostTag postTag in postTags)
                        {
                            Tag? tag = await _tagRepo.Get(postTag.TagId);
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
                    }

                    PostInfoModel postInfo = new()
                    {
                        PostInfo = postModel,
                        PostCategories = postCategoryModels,
                        PostTags = postTagModels
                    };
                    listPosts.Add(postInfo);
                }


                Page<PostInfoModel> postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);

                PaginationResponseModel paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                    .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount).RecordCount(postPaged.RecordCount);
                responseModel.Paging = paging;
                responseModel.ListPosts = postPaged.Results;
            }

            return responseModel;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> GetTopLiked()
    {
        try
        {
            List<Post>? posts = await _postRepo.GetTopLiked();
            List<PostInfoModel> listPosts = new();
            if (posts != null && posts.Any())
            {
                foreach (Post post in posts)
                {
                    PostAuthor? author = await _userRepo.GetAuthor(post.Author);
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
                        Status = post.Status,
                        AdminStatus = post.AdminStatus
                    };
                    List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                    List<PostCategoryModel> postCategoryModels = new();
                    if (postCategories.Any())
                    {
                        foreach (PostCategory cate in postCategories)
                        {
                            Category? category = await _categoryRepo.Get(cate.CategoryId);
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
                    }

                    List<PostTag> postTags = await _postTagRepo.GetPostTags(post.Id);
                    List<PostTagModel> postTagModels = new();
                    if (postTags.Any())
                    {
                        foreach (PostTag postTag in postTags)
                        {
                            Tag? tag = await _tagRepo.Get(postTag.TagId);
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
                    }

                    PostInfoModel postInfo = new()
                    {
                        PostInfo = postModel,
                        PostCategories = postCategoryModels,
                        PostTags = postTagModels
                    };
                    listPosts.Add(postInfo);
                }
            }

            return listPosts;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> SearchPosts(int pageNumber, int pageSize, string postName, string? status,
        string? adminStatus,
        Guid? categoryId, Guid? tagId)
    {
        try
        {
            status ??= "all";
            adminStatus ??= "approved";
            List<Post>? posts = await _postRepo.SearchPost(postName, status, adminStatus);
            PostPagingResponseModel responseModel = new();
            if (posts != null && posts.Any())
            {
                List<PostInfoModel> listPosts = new();
                foreach (Post post in posts)
                {
                    PostAuthor? author = await _userRepo.GetAuthor(post.Author);
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
                        Status = post.Status,
                        AdminStatus = post.AdminStatus
                    };
                    List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                    List<PostCategoryModel> postCategoryModels = new();
                    if (postCategories.Any())
                    {
                        foreach (PostCategory cate in postCategories)
                        {
                            Category? category = await _categoryRepo.Get(cate.CategoryId);
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
                    }

                    List<PostTag> postTags = await _postTagRepo.GetPostTags(post.Id);
                    List<PostTagModel> postTagModels = new();
                    if (postTags.Any())
                    {
                        foreach (PostTag postTag in postTags)
                        {
                            Tag? tag = await _tagRepo.Get(postTag.TagId);
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
                    }

                    PostInfoModel postInfo = new()
                    {
                        PostInfo = postModel,
                        PostCategories = postCategoryModels,
                        PostTags = postTagModels
                    };
                    listPosts.Add(postInfo);
                }

                if (categoryId != null && categoryId != Guid.Empty)
                {
                    List<PostInfoModel> newListPosts = new();
                    foreach (PostInfoModel post in listPosts)
                    {
                        foreach (PostCategoryModel postCate in post.PostCategories)
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

                if (tagId != null && tagId != Guid.Empty)
                {
                    List<PostInfoModel> newListPosts = new();
                    foreach (PostInfoModel post in listPosts)
                    {
                        foreach (PostTagModel postTag in post.PostTags)
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

                Page<PostInfoModel> postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);

                PaginationResponseModel paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                    .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount == 0 ? 1 : postPaged.PageCount)
                    .RecordCount(postPaged.RecordCount);
                responseModel.Paging = paging;
                responseModel.ListPosts = postPaged.Results;
            }
            else
            {
                List<PostInfoModel> listPosts = new();
                if (listPosts == null)
                {
                    throw new ArgumentNullException(nameof(listPosts));
                }

                Page<PostInfoModel> postPaged = listPosts.AsQueryable().Paginate(pageNumber, pageSize);
                PaginationResponseModel paging = new PaginationResponseModel().CurPage(postPaged.CurrentPage)
                    .PageSize(postPaged.PageSize).PageCount(postPaged.PageCount == 0 ? 1 : postPaged.PageCount)
                    .RecordCount(postPaged.RecordCount);
                responseModel.Paging = paging;
                responseModel.ListPosts = postPaged.Results;
            }

            return responseModel;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> UpdatePost(PostUpdateModel postUpdateModel, string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            Post? postCheck = await _postRepo.Get(postUpdateModel.Id);
            if (postCheck == null)
            {
                return "Post not found!";
            }

            if (!postCheck.Author.Equals(Guid.Parse(userId)))
            {
                return "Not the author!";
            }

            bool result = await _postRepo.UpdatePost(postUpdateModel);
            if (result)
            {
                _ = await _cacheService.RemoveOldCache("post");
                object? post = await GetPost(postUpdateModel.Id);
                return post ?? "Something went wrong!";
            }

            return "Something went wrong!";
        }
        catch (Exception? e)
        {
            return e;
        }
    }

    public async Task<object> ChangePostStatus(Guid postId, string status)
    {
        try
        {
            bool result = false;
            if (status.Trim().ToLower().Equals("active"))
            {
                result = await _postRepo.ChangePostStatus(postId, true);
            }

            if (status.Trim().ToLower().Equals("inactive"))
            {
                result = await _postRepo.ChangePostStatus(postId, false);
            }

            if (result)
            {
                object? post = await GetPost(postId);
                if (post != null)
                {
                    _ = await _cacheService.RemoveOldCache("post");
                    return post;
                }

                return "SomeThing went wrong!";
            }

            return "SomeThing went wrong!";
        }
        catch (Exception? e)
        {
            return e;
        }
    }

    public async Task<object> DeletePost(Guid postId)
    {
        try
        {
            Post? post = await _postRepo.Get(postId);
            if (post == null)
            {
                return "Post not found!";
            }

            List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(postId);
            if (postCategories.Any())
            {
                foreach (PostCategory postCate in postCategories)
                {
                    _ = await _postCategoryRepo.Delete(postCate);
                }
            }

            List<PostTag> postTags = await _postTagRepo.GetPostTags(postId);
            if (postTags.Any())
            {
                foreach (PostTag postTag in postTags)
                {
                    _ = await _postTagRepo.Delete(postTag);
                }
            }

            int result = await _postRepo.Delete(post);
            if (result > 0)
            {
                _ = await _cacheService.RemoveOldCache("post");
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object?> GetRelatedPosts(string relatePostId)
    {
        try
        {
            List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(Guid.Parse(relatePostId));
            List<PostCategoryModel> postCategoryModels = new();
            if (postCategories.Any())
            {
                foreach (PostCategory cate in postCategories)
                {
                    Category? category = await _categoryRepo.Get(cate.CategoryId);
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
            }

            List<string> postIds = new();
            foreach (PostCategoryModel category in postCategoryModels)
            {
                List<string>? listPosts = await _postRepo.GetPostsByCategory(category.Id);
                if (listPosts != null)
                {
                    foreach (string listPostCate in listPosts)
                    {
                        postIds.Add(listPostCate);
                    }
                }
            }

            postIds = postIds.Distinct().Take(6).ToList();
            List<PostInfoModel> response = new();
            foreach (string postId in postIds)
            {
                object? postInfo = await GetPost(Guid.Parse(postId));
                if (postInfo?.GetType() == typeof(PostInfoModel))
                {
                    response.Add((PostInfoModel)postInfo);
                }
            }

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<object?> LikePost(string postId, string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            _ = await _cacheService.RemoveOldCache("post");
            return await _postRepo.LikePost(postId, userId);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object?> DislikePost(string postId, string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            _ = await _cacheService.RemoveOldCache("post");
            return await _postRepo.DislikePost(postId, userId);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> BanPost(string postId)
    {
        try
        {
            _ = await _cacheService.RemoveOldCache("post");
            return await _postRepo.BanPost(postId);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> ApprovePost(string postId)
    {
        try
        {
            _ = await _cacheService.RemoveOldCache("post");
            return await _postRepo.ApprovePost(postId);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> UnbanPost(string postId)
    {
        try
        {
            _ = await _cacheService.RemoveOldCache("post");
            return await _postRepo.UnBanPost(postId);
        }
        catch (Exception e)
        {
            return e;
        }
    }
}