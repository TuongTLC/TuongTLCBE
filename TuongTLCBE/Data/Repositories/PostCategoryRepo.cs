using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class PostCategoryRepo : Repository<PostCategory>
{
    public PostCategoryRepo(TuongTlcdbContext context) : base(context)
    {
    }

    public async Task<List<PostCategory>> GetPostCategories(Guid postId)
    {
        return await context.PostCategories.Where(x => x.PostId.Equals(postId)).ToListAsync();
    }

    public async Task<bool> UpdatePostCategories(Guid postId, List<Guid> categoriesIds)
    {
        try
        {
            List<PostCategory> postCategories =
                await context.PostCategories.Where(x => x.PostId.Equals(postId)).ToListAsync();
            if (postCategories.Any())
                foreach (var pc in postCategories)
                    context.Remove(pc);

            foreach (var cateId in categoriesIds)
                context.Add(new PostCategory() { Id = Guid.NewGuid(), PostId = postId, CategoryId = cateId });

            _ = await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePostCategory(Guid postId, Guid categoryId)
    {
        try
        {
            var postCategory = await context.PostCategories
                .Where(x => x.PostId.Equals(postId) && x.CategoryId.Equals(categoryId)).FirstOrDefaultAsync();
            if (postCategory != null)
            {
                context.Remove(postCategory);
                await context.SaveChangesAsync();
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