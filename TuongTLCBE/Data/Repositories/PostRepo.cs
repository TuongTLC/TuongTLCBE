using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class PostRepo : Repository<Post>
{
    private readonly PostCategoryRepo _postCategoryRepo;
    private readonly PostTagRepo _postTagRepo;
    private readonly UserInteractPostRepo _userInteractPostRepo;

    public PostRepo(TuongTlcdbContext context, PostCategoryRepo postCategoryRepo, PostTagRepo postTagRepo,
        UserInteractPostRepo userInteractPostRepo) :
        base(context)
    {
        _postCategoryRepo = postCategoryRepo;
        _postTagRepo = postTagRepo;
        _userInteractPostRepo = userInteractPostRepo;
    }

    public async Task<List<string>?> GetPostsByCategory(Guid categoryIds)
    {
        try
        {
            var query = from p in context.Posts
                join pc in context.PostCategories on p.Id equals pc.PostId
                join c in context.Categories on pc.CategoryId equals c.Id
                where c.Id.Equals(categoryIds) && p.Status == true && p.AdminStatus.Equals(Enums.POST_APPROVED)
                orderby p.Like descending
                select new { PostId = p.Id };
            var posts = await query.Select(x => new string(x.PostId.ToString())).Take(6).ToListAsync();
            return posts;
        }
        catch
        {
            return null;
        }
    }


    public async Task<List<Post>?> GetPostsInfo(string status, string adminStatus)
    {
        try
        {
            if (adminStatus.Equals("all"))
                switch (status)
                {
                    case "all":
                        return await context.Posts
                            .OrderByDescending(x => x.CreateDate)
                            .ToListAsync();
                    case "active":
                        return await context.Posts.Where(x =>
                                x.Status == true)
                            .OrderByDescending(x => x.CreateDate)
                            .ToListAsync();
                    case "inactive":
                        return await context.Posts.Where(x =>
                                x.Status == false)
                            .OrderByDescending(x => x.CreateDate)
                            .ToListAsync();
                    default:
                        return await context.Posts.OrderByDescending(x => x.CreateDate).ToListAsync();
                }

            switch (status)
            {
                case "all":
                    return await context.Posts.Where(y => y.AdminStatus.Equals(adminStatus.Trim().ToLower()))
                        .OrderByDescending(x => x.CreateDate)
                        .ToListAsync();
                case "active":
                    return await context.Posts.Where(x =>
                            x.Status == true && x.AdminStatus.Equals(adminStatus.Trim().ToLower()))
                        .OrderByDescending(x => x.CreateDate)
                        .ToListAsync();
                case "inactive":
                    return await context.Posts.Where(x =>
                            x.Status == false && x.AdminStatus.Equals(adminStatus.Trim().ToLower()))
                        .OrderByDescending(x => x.CreateDate)
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

    public async Task<List<Post>?> SearchPost(string postName, string status, string adminStatus)
    {
        try
        {
            switch (status)
            {
                case "all":
                    return await context.Posts.Where(x =>
                            x.PostName.Contains(postName) && x.AdminStatus.Equals(adminStatus.Trim().ToLower()))
                        .OrderByDescending(x => x.CreateDate).ToListAsync();

                case "active":
                    return await context.Posts.Where(z => z.PostName.Contains(postName))
                        .Where(x => x.Status.Equals(true) && x.AdminStatus.Equals(adminStatus.Trim().ToLower()))
                        .OrderByDescending(x => x.CreateDate).ToListAsync();

                case "inactive":
                    return await context.Posts.Where(z => z.PostName.Contains(postName))
                        .Where(x => x.Status.Equals(false) && x.AdminStatus.Equals(adminStatus.Trim().ToLower()))
                        .OrderByDescending(x => x.CreateDate).ToListAsync();
                default:
                    return await context.Posts.Where(x =>
                            x.PostName.Contains(postName) && x.AdminStatus.Equals(adminStatus.Trim().ToLower()))
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
                .ThenByDescending(y => y.Status)
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
                var categoryUpdate = false;
                var tagUpdate = false;
                if (postUpdateModel.CategoriesIds.Any())
                    categoryUpdate =
                        await _postCategoryRepo.UpdatePostCategories(postUpdateModel.Id, postUpdateModel.CategoriesIds);

                if (postUpdateModel.TagsIds.Any())
                    tagUpdate = await _postTagRepo.UpdatePostTags(postUpdateModel.Id, postUpdateModel.TagsIds);

                if (categoryUpdate == false || tagUpdate == false) return false;
                post.PostName = postUpdateModel.PostName;
                post.Summary = postUpdateModel.Summary;
                post.Content = postUpdateModel.Content;
                post.Thumbnail = postUpdateModel.Thumbnail;
                var postUpdate = await context.SaveChangesAsync();
                if (postUpdate == 0)
                {
                    if (categoryUpdate || tagUpdate) return true;
                    return false;
                }

                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }


    public async Task<bool?> LikePost(string postId, string userId)
    {
        try
        {
            var checkInteraction = await context.UserInteractPosts
                .Where(x => x.UserId.Equals(Guid.Parse(userId)) &&
                            x.PostId.Equals(Guid.Parse(postId)))
                .FirstOrDefaultAsync();
            if (checkInteraction != null) return null;

            var post = await context.Posts.Where(x => x.Id.Equals(Guid.Parse(postId)))
                .FirstOrDefaultAsync();
            if (post != null)
            {
                post.Like += 1;
                var update = await context.SaveChangesAsync();
                if (update > 0)
                    _ = await _userInteractPostRepo.Insert(new UserInteractPost
                        { Id = Guid.NewGuid(), UserId = Guid.Parse(userId), PostId = Guid.Parse(postId) });
                if (update == 0) return false;
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool?> DislikePost(string postId, string userId)
    {
        try
        {
            var checkInteraction = await context.UserInteractPosts
                .Where(x => x.UserId.Equals(Guid.Parse(userId)) &&
                            x.PostId.Equals(Guid.Parse(postId)))
                .FirstOrDefaultAsync();
            if (checkInteraction != null) return null;

            var comment = await context.Posts.Where(x => x.Id.Equals(Guid.Parse(postId)))
                .FirstOrDefaultAsync();
            if (comment != null)
            {
                comment.Dislike += 1;
                var update = await context.SaveChangesAsync();
                if (update > 0)
                    _ = await _userInteractPostRepo.Insert(new UserInteractPost
                        { Id = Guid.NewGuid(), UserId = Guid.Parse(userId), PostId = Guid.Parse(postId) });
                if (update == 0) return false;
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> BanPost(string postId)
    {
        try
        {
            var post = await context.Posts.Where(x => x.Id.Equals(Guid.Parse(postId)))
                .FirstOrDefaultAsync();
            if (post != null)
            {
                post.AdminStatus = Enums.POST_BANNED;
                post.Status = false;
                var update = await context.SaveChangesAsync();
                if (update == 0) return false;
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ApprovePost(string postId)
    {
        try
        {
            var post = await context.Posts.Where(x => x.Id.Equals(Guid.Parse(postId)))
                .FirstOrDefaultAsync();
            if (post != null)
            {
                post.AdminStatus = Enums.POST_APPROVED;
                post.Status = true;
                var update = await context.SaveChangesAsync();
                if (update == 0) return false;
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UnBanPost(string postId)
    {
        try
        {
            var post = await context.Posts.Where(x => x.Id.Equals(Guid.Parse(postId)))
                .FirstOrDefaultAsync();
            if (post != null)
            {
                post.AdminStatus = Enums.POST_APPROVED;
                var update = await context.SaveChangesAsync();
                if (update == 0) return false;
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