using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
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
                _ = context.Update(category);
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
    public async Task<bool> DisableCategory(Guid categoryId)
    {
        try
        {
            Category? category = await context.Categories.Where(x => x.Id.Equals(categoryId))
                .FirstOrDefaultAsync();
            if (category != null)
            {
                category.Status = false;
                _ = context.Update(category);
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