using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class PostRepo : Repository<Post>
{
    public PostRepo(TuongTlcdbContext context) : base(context)
    {
    }


    public async Task<List<Post>?> GetPostsInfo(string status)
    {
        try
        {
            switch (status)
            {
                case "all":
                    return await context.Posts.OrderByDescending(x => x.CreateDate).ToListAsync();
                case "active":
                    return await context.Posts.Where(x => x.Status.Equals(true)).OrderByDescending(x => x.CreateDate)
                        .ToListAsync();
                case "inactive":
                    return await context.Posts.Where(x => x.Status.Equals(false)).OrderByDescending(x => x.CreateDate)
                        .ToListAsync();
                default:
                    return await context.Posts.OrderByDescending(x => x.CreateDate).ToListAsync();
            }
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Post>?> SearchPost(string postName, string? status)
    {
        try
        {
            switch (status)
            {
                case "all":
                    return await context.Posts.Where(x => x.PostName.Contains(postName))
                        .OrderByDescending(x => x.CreateDate).ToListAsync();

                case "active":
                    return await context.Posts.Where(z => z.PostName.Contains(postName))
                        .Where(x => x.Status.Equals(true)).OrderByDescending(x => x.CreateDate).ToListAsync();

                case "inactive":
                    return await context.Posts.Where(z => z.PostName.Contains(postName))
                        .Where(x => x.Status.Equals(false)).OrderByDescending(x => x.CreateDate).ToListAsync();
                default:
                    return await context.Posts.Where(x => x.PostName.Contains(postName))
                        .OrderByDescending(x => x.CreateDate).ToListAsync();
            }
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Post>?> GetPostByUser(Guid userId)
    {
        try
        {
            return await context.Posts.Where(x => x.Author.Equals(userId)).OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Post>?> GetTopLiked()
    {
        try
        {
            return await context.Posts.OrderByDescending(x => x.Like).Take(3)
                .ToListAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ChangePostStatus(Guid postId, bool status)
    {
        try
        {
            var post = await context.Posts.Where(x => x.Id.Equals(postId)).FirstOrDefaultAsync();
            if (post != null)
            {
                post.Status = status;
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