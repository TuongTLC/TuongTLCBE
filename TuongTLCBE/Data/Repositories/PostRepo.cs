using EntityFrameworkPaginateCore;
using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class PostRepo: Repository<Post>
{
    private readonly PostCategoryRepo _postCategoryRepo;
    private readonly PostTagRepo _postTagRepo;
    private readonly CategoryRepo _categoryRepo;
    private readonly TagRepo _tagRepo;
    public PostRepo(TuongTlcdbContext context, PostCategoryRepo postCategoryRepo, PostTagRepo postTagRepo, CategoryRepo categoryRepo, TagRepo tagRepo) : base(context)
    {
        _postCategoryRepo = postCategoryRepo;
        _postTagRepo = postTagRepo;
        _categoryRepo = categoryRepo;
        _tagRepo = tagRepo;
    }

    public async Task<PostInfoModel?> GetPostInfo(Guid postId) 
    {
        try
        {
            Post? post = await context.Posts.Where(x => x.Id.Equals(postId)).FirstOrDefaultAsync();
            if (post != null)
            {
                PostModel postModel = new()
                {
                    Id = post.Id,
                    PostName = post.PostName,
                    Summary = post.Summary,
                    Content = post.Content,
                    CreateDate = post.CreateDate,
                    Author = post.Author,
                    Like = post.Like,
                    Dislike = post.Dislike,
                    Thumbnail = post.Thumbnail,
                    Status = post.Status
                };
                List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(postId);
                List<PostCategoryModel> postCategoryModels = new();
                if (postCategories.Any())
                {
                    foreach (var cate in postCategories)
                    {
                        Category? category = await _categoryRepo.Get(cate.CategoryId);
                        if (category != null)
                        {
                            PostCategoryModel model= new()
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
                    foreach (var postTag in postTags)
                    {
                        Tag? tag = await _tagRepo.Get(postTag.TagId);
                        if (tag != null)
                        {
                            PostTagModel model= new()
                            {
                                Id = tag.Id,
                                TagName = tag.TagName,
                                Description = tag.Description
                            };
                            postTagModels.Add(model);
                        }
                    }
                }

                PostInfoModel postInfo = new PostInfoModel()
                {
                    PostInfo = postModel,
                    PostCategories = postCategoryModels,
                    PostTags = postTagModels
                };
                return postInfo;
            }
            else
            {
                return null;
            }
        }
        catch 
        {
            return null;
        }
    }
    public async Task<object?> GetPostsInfo(int pageNumber, int pageSize)
    {
        try
        {
            Page<Post> posts = await context.Posts.OrderByDescending(x => x.CreateDate)
                .PaginateAsync(pageNumber, pageSize);
            if (posts != null)
            {
                List<PostInfoModel> listPosts = new();
                foreach (var post in posts.Results)
                {
                    PostModel postModel = new()
                    {
                        Id = post.Id,
                        PostName = post.PostName,
                        Summary = post.Summary,
                        Content = post.Content,
                        CreateDate = post.CreateDate,
                        Author = post.Author,
                        Like = post.Like,
                        Dislike = post.Dislike,
                        Thumbnail = post.Thumbnail,
                        Status = post.Status
                    };
                    List<PostCategory> postCategories = await _postCategoryRepo.GetPostCategories(post.Id);
                    List<PostCategoryModel> postCategoryModels = new();
                    if (postCategories.Any())
                    {
                        foreach (var cate in postCategories)
                        {
                            Category? category = await _categoryRepo.Get(cate.CategoryId);
                            if (category != null)
                            {
                                PostCategoryModel postCategoryModel= new()
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
                        foreach (var postTag in postTags)
                        {
                            Tag? tag = await _tagRepo.Get(postTag.TagId);
                            if (tag != null)
                            {
                                PostTagModel postTagModel= new()
                                {
                                    Id = tag.Id,
                                    TagName = tag.TagName,
                                    Description = tag.Description
                                };
                                postTagModels.Add(postTagModel);
                            }
                        }
                    }

                    PostInfoModel postInfo = new PostInfoModel()
                    {
                        PostInfo = postModel,
                        PostCategories = postCategoryModels,
                        PostTags = postTagModels
                    };
                    listPosts.Add(postInfo);
                }

                PaginationResponseModel paging = new PaginationResponseModel().CurPage(posts.CurrentPage)
                    .PageSize(posts.PageSize).PageCount(posts.PageCount).RecordCount(posts.RecordCount);
                PostPagingResponseModel model = new()
                {
                    Paging = paging,
                    ListPosts = listPosts
                };
                return model;
            }
            else
            {
                return null;
            }
        }
        catch 
        {
            return null;
        }
    }

    public async Task<bool> ChangePostStatus(Guid postId, string status)
    {
        try
        {
            bool stat;
            if ( status.Trim().ToLower().Equals("active"))
            {
                stat = true;
            }else if ( status.Trim().ToLower().Equals("inactive"))
            {
                stat = true;
            }
            else
            {
                return false;
            }

            Post? post = await context.Posts.Where(x => x.Id.Equals(postId)).FirstOrDefaultAsync();
            if (post != null)
            {
                post.Status = stat;
                _ = await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
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
            Post? post = await context.Posts.Where(x => x.Id.Equals(postUpdateModel.Id)).FirstOrDefaultAsync();
            if (post != null)
            {
                post.PostName = postUpdateModel.PostName;
                post.Summary = postUpdateModel.Summary;
                post.Content = postUpdateModel.Content;
                post.Thumbnail = postUpdateModel.Thumbnail;
                _ = await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
}