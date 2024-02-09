using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class PostCategoryService
{
    private readonly PostCategoryRepo _postCategoryRepo;

    public PostCategoryService(PostCategoryRepo postCategoryRepo)
    {
        _postCategoryRepo = postCategoryRepo;
    }

    public async Task<object?> GetPostCategories(Guid postId)
    {
        try
        {
            return await _postCategoryRepo.GetPostCategories(postId);
        }
        catch (Exception? e)
        {
            return e;
        }
    }
    public async Task<object> InsertPostCategory(Guid postId, List<Guid> categoriesIds)
    {
        try
        {
            if (categoriesIds.Any())
            {
                foreach (var cateId in categoriesIds)
                {
                    PostCategory postCategory = new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = postId,
                        CategoryId = cateId
                    };
                    await _postCategoryRepo.Insert(postCategory);
                }
                return _postCategoryRepo.GetPostCategories(postId);
            }
            else
            {
                return "Categories ID is empty!";
            }
            
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> UpdatePostCategory(Guid postId, List<Guid> categoriesIds)
    {
        try
        {
            if (categoriesIds.Any())
            {
                bool result = await _postCategoryRepo.UpdatePostCategories(postId, categoriesIds);
                if (result)
                {
                    return _postCategoryRepo.GetPostCategories(postId);
                }
                else
                {
                    return "Update post's categories failed!";
                }
            }
            else
            {
                return "No category id to update!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> DeletePostCategory(Guid postId, Guid categoryId)
    {
        try
        {
            bool result = await _postCategoryRepo.DeletePostCategory(postId, categoryId);
            if (result)
            {
                return _postCategoryRepo.GetPostCategories(postId);
            }
            else
            {
                return "Delete post category failed!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }
}