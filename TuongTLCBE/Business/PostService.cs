using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class PostService
{
    private readonly PostCategoryRepo _postCategoryRepo;
    private readonly PostTagRepo _postTagRepo;
    private readonly CategoryRepo _categoryRepo;
    private readonly TagRepo _tagRepo;
    private readonly PostRepo _postRepo;
    private readonly DecodeToken _decodeToken;
    public PostService(DecodeToken decodeToken, PostCategoryRepo postCategoryRepo, PostTagRepo postTagRepo, CategoryRepo categoryRepo, TagRepo tagRepo, PostRepo postRepo)
    {
        _postCategoryRepo = postCategoryRepo;
        _postTagRepo = postTagRepo;
        _categoryRepo = categoryRepo;
        _tagRepo = tagRepo;
        _postRepo = postRepo;
        _decodeToken = decodeToken;
    }


    public async Task<object> InsertPost(PostRequestModel postRequestModel, string token)
    {
        try
        {
            if (postRequestModel.PostName.Length <=6 )
            {
                return "Post name too short!";
            }
            if (postRequestModel.Content.Length <= 50 )
            {
                return "Content too short!";
            }
            if (!postRequestModel.CategoriesIds.Any() && !postRequestModel.TagsIds.Any())
            {
                return "Categories id or tag id is empty!";
            }

            foreach (var cateId in postRequestModel.CategoriesIds)
            {
                Category? categoryCheck = await _categoryRepo.Get(cateId);
                if (categoryCheck == null)
                {
                    return "Category " + cateId +" not found!";
                }
            }
            foreach (var tagId in postRequestModel.TagsIds)
            {
                Tag? tagCheck = await _tagRepo.Get(tagId);
                if (tagCheck == null)
                {
                    return "Tag " + tagId +" not found!";
                }
            }

            string userId = _decodeToken.Decode(token, "userid");
            Post post = new Post()
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
                Status = false
            };
            Post? insertPost = await _postRepo.Insert(post);
            if (insertPost != null)
            {
                foreach (var cateInsert in postRequestModel.CategoriesIds)
                {
                    PostCategory postCategoryInsert = new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        CategoryId = cateInsert
                    };
                    _ = await _postCategoryRepo.Insert(postCategoryInsert);
                }
                foreach (var tagInsert in postRequestModel.TagsIds)
                {
                    PostTag postTagInsert = new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        TagId = tagInsert
                    };
                    _ = await _postTagRepo.Insert(postTagInsert);
                }

                PostInfoModel? postInfoModel = await _postRepo.GetPostInfo(post.Id);
                return (postInfoModel != null) ? postInfoModel : "Something went wrong!!";
            }
            else
            {
                return "Insert post failed!";
            }

        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> GetPost(Guid postId)
    {
        try
        {
            PostInfoModel? postInfoModel = await _postRepo.GetPostInfo(postId);
            return postInfoModel != null ? postInfoModel : "Something went Wrong";
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> GetPosts(int pageNumber, int pageSize, string? status, Guid? categoryId, Guid? tagId)
    {
        try
        {
            object? posts = await _postRepo.GetPostsInfo(pageNumber, pageSize, status, categoryId, tagId);
            return posts;
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
                PostInfoModel? post = await _postRepo.GetPostInfo(postUpdateModel.Id);
                if (post != null)
                {
                    return post;
                }
                else
                {
                    return "SomeThing went wrong!";
                }
            }
            else
            {
                return "SomeThing went wrong!";
            }
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
            bool result = await _postRepo.ChangePostStatus(postId, status);
            if (result)
            {
                PostInfoModel? post = await _postRepo.GetPostInfo(postId);
                if (post != null)
                {
                    return post;
                }
                else
                {
                    return "SomeThing went wrong!";
                }
            }
            else
            {
                return "SomeThing went wrong!";
            }
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
                foreach (var postCate in postCategories)
                {
                   await _postCategoryRepo.Delete(postCate);
                }
            }

            List<PostTag> postTags = await _postTagRepo.GetPostTags(postId);
            if (postTags.Any())
            {
                foreach (var postTag in postTags)
                {
                    await _postTagRepo.Delete(postTag);
                }
            }

            int result = await _postRepo.Delete(post);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }
}