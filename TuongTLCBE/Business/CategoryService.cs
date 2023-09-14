using Microsoft.IdentityModel.Tokens;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class CategoryService
{
    private readonly CategoryRepo _categoryRepo;
    private readonly DecodeToken _decodeToken;

    public CategoryService(CategoryRepo categoryRepo, DecodeToken decodeToken)
    {
        _categoryRepo = categoryRepo;
        _decodeToken = decodeToken;
    }

    public async Task<object> CreateCategory(CategoryInsertModel categoryInsertModel, string token)
    {
        try
        {
            string userID = _decodeToken.Decode(token, "userid");
            if (categoryInsertModel.CategoryName.IsNullOrEmpty())
            {
                return "Category name invalid!";
            }

            Category insertCategory = new()
            {
                Id = Guid.NewGuid(),
                CategoryName = categoryInsertModel.CategoryName,
                Description = categoryInsertModel.Description,
                CreatedBy = Guid.Parse(userID),
                CreatedDate = DateTime.Now
            };
            Category? insert = await _categoryRepo.Insert(insertCategory);
            if (insert != null)
            {
                CategoryModel res = new()
                {
                    Id = insert.Id,
                    CategoryName = insert.CategoryName,
                    Description = insert.Description,
                    CreatedBy = insert.CreatedBy,
                    CreatedDate = insert.CreatedDate
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
    public async Task<object> GetCategories()
    {
        try
        {
            var categoryList = await _categoryRepo.GetList();
            if (categoryList.Any())
            {
                List<CategoryModel> resList = new();
                foreach (var cate in categoryList)
                { 
                    CategoryModel resCate = new()
                    {
                        Id = cate.Id,
                        CategoryName = cate.CategoryName,
                        Description = cate.Description,
                        CreatedBy = cate.CreatedBy,
                        CreatedDate = cate.CreatedDate
                    };
                    resList.Add(resCate);
                } 
                return resList;
            }
            else
            {
                return "Get list categories failed!";
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
                        CategoryName = res.CategoryName,
                        Description = res.Description,
                        CreatedBy = res.CreatedBy,
                        CreatedDate = res.CreatedDate
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
}