using Microsoft.IdentityModel.Tokens;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class CategoryService(CategoryRepo categoryRepo, DecodeToken decodeToken)
{
    private readonly CategoryRepo _categoryRepo = categoryRepo;
    private readonly DecodeToken _decodeToken = decodeToken;

    public async Task<object> CreateCategory(CategoryInsertModel categoryInsertModel, string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            if (categoryInsertModel.CategoryName.IsNullOrEmpty())
            {
                return "Category name invalid!";
            }

            Category insertCategory = new()
            {
                Id = Guid.NewGuid(),
                CategoryName = categoryInsertModel.CategoryName,
                Description = categoryInsertModel.Description,
                CreatedBy = Guid.Parse(userId),
                CreatedDate = DateTime.Now
            };
            Category? insert = await _categoryRepo.Insert(insertCategory);
            if (insert != null)
            {
                CategoryModel res = new()
                {
                    Id = insert.Id,
                    CategoryName = insert.CategoryName ?? string.Empty,
                    Description = insert.Description,
                    CreatedBy = insert.CreatedBy ?? Guid.Empty,
                    CreatedDate = insert.CreatedDate ?? DateTime.MinValue,
                    Status = insert.Status
                };
                return res;
            }
            else
            {
                return "Create category failed!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> UpdateCategory(CategoryUpdateModel categoryUpdateModel)
    {
        try
        {
            bool update = await _categoryRepo.UpdateCategory(categoryUpdateModel);
            if (update)
            {
                Category? res = await _categoryRepo.Get(categoryUpdateModel.Id);
                if (res != null)
                {
                    CategoryModel resCate = new()
                    {
                        Id = res.Id,
                        CategoryName = res.CategoryName ?? string.Empty,
                        Description = res.Description,
                        CreatedBy = res.CreatedBy ?? Guid.Empty,
                        CreatedDate = res.CreatedDate ?? DateTime.MinValue,
                        Status = res.Status
                    };
                    return resCate;
                }
                else
                {
                    return "Failed to retrieve updated category!";
                }
            }
            else
            {
                return "Failed to update category!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> ChangeCategoryStatus(Guid categoryId, bool status)
    {
        try
        {
            bool result = await _categoryRepo.ChangeCategoryStatus(categoryId, status);
            return result ? true : "Change category status failed!";
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> DeleteCategory(Guid categoryId)
    {
        try
        {
            Category? getCate = await _categoryRepo.Get(categoryId);
            if (getCate != null)
            {
                int result = await _categoryRepo.Delete(getCate);
                return result > 0 ? true : "Delete category failed!";
            }
            else
            {
                return "Category not exist!";
            }
        }
        catch
        {
            return "Category still in use!";
        }
    }

    public async Task<object> GetACategory(Guid categoryId)
    {
        try
        {
            CategoryModel? categoryModel = await _categoryRepo.GetACategory(categoryId);
            return categoryModel != null ? categoryModel : "Category not found!";
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> GetCategories(string? status)
    {
        try
        {
            List<CategoryModel>? categoryModel = await _categoryRepo.GetCategories(status);
            return categoryModel != null ? categoryModel : "Error while getting categories!";
        }
        catch (Exception e)
        {
            return e;
        }
    }
}