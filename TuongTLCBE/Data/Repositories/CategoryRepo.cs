using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class CategoryRepo: Repository<Category>
{
    public CategoryRepo(TuongTlcdbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateCategory(CategoryUpdateModel categoryUpdateModel)
    {
        try
        {
            Category? category = await context.Categories.Where(x => x.Id.Equals(categoryUpdateModel.Id))
                .FirstOrDefaultAsync();
            if (category != null)
            {
                category.CategoryName = categoryUpdateModel.CategoryName;
                category.Description = categoryUpdateModel.Description;
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
    public async Task<bool> ChangeCategoryStatus(Guid categoryId, bool status)
    {
        try
        {
            Category? category = await context.Categories.Where(x => x.Id.Equals(categoryId))
                .FirstOrDefaultAsync();
            if (category != null)
            {
                category.Status = status;
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
    public async Task<CategoryModel?> GetACategory(Guid categoryId)
    {
        try
        {
            var query = from c in context.Categories
                where c.Id.Equals(categoryId)
                select new { c };
            CategoryModel? categoryModel = await query.Select(x => new CategoryModel()
            {
                Id = x.c.Id,
                CategoryName = x.c.CategoryName,
                Description = x.c.Description,
                CreatedBy = x.c.CreatedBy,
                CreatedDate = x.c.CreatedDate,
                Status = x.c.Status
            }).FirstOrDefaultAsync();
            return categoryModel;
        }
        catch 
        {
            return null;
        }
    }
    public async Task<List<CategoryModel>?> GetCategories(string? status)
    {
        try
        {
            if (!status.IsNullOrEmpty())
            {
                bool statusIn = true;
                if (status != null && status.Equals("active"))
                {
                    statusIn = true;
                }
                if (status != null && status.Equals("inactive"))
                {
                    statusIn = false;
                }
                var query = from c in context.Categories
                    where c.Status.Equals(statusIn)
                    select new { c };
                List<CategoryModel> categoryModel = await query.Select(x => new CategoryModel()
                {
                    Id = x.c.Id,
                    CategoryName = x.c.CategoryName,
                    Description = x.c.Description,
                    CreatedBy = x.c.CreatedBy,
                    CreatedDate = x.c.CreatedDate,
                    Status = x.c.Status
                }).ToListAsync();
                return categoryModel;
            }
            else
            {
                var query = from c in context.Categories
                    select new { c };
                List<CategoryModel> categoryModel = await query.Select(x => new CategoryModel()
                {
                    Id = x.c.Id,
                    CategoryName = x.c.CategoryName,
                    Description = x.c.Description,
                    CreatedBy = x.c.CreatedBy,
                    CreatedDate = x.c.CreatedDate,
                    Status = x.c.Status
                }).ToListAsync();
                return categoryModel;
            }
        }
        catch 
        {
            return null;
        }
    }
}