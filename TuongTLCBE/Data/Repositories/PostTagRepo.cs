using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class PostTagRepo : Repository<PostTag>
{
    public PostTagRepo(TuongTLCDBContext context) : base(context)
    {
    }

    public async Task<List<PostTag>> GetPostTags(Guid postId)
    {
        return await context.PostTags.Where(x => x.PostId.Equals(postId)).ToListAsync();
    }

    public async Task<bool> UpdatePostTags(Guid postId, List<Guid> tagsIds)
    {
        try
        {
            List<PostTag> postTags =
                await context.PostTags.Where(x => x.PostId.Equals(postId)).ToListAsync();
            if (postTags.Any())
            {
                foreach (PostTag pc in postTags)
                {
                    _ = context.Remove(pc);
                }
            }

            foreach (Guid cateId in tagsIds)
            {
                _ = context.Add(new PostTag() { Id = Guid.NewGuid(), PostId = postId, TagId = cateId });
            }

            _ = await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePostTag(Guid postId, Guid tagId)
    {
        try
        {
            PostTag? postTag = await context.PostTags
                .Where(x => x.PostId.Equals(postId) && x.TagId.Equals(tagId)).FirstOrDefaultAsync();
            if (postTag != null)
            {
                _ = context.Remove(postTag);
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