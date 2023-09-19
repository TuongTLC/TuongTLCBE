using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class PostTagRepo: Repository<PostTag>
{
    public PostTagRepo(TuongTlcdbContext context) : base(context)
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
                foreach (var pc in postTags)
                {
                    context.Remove(pc);
                }
            }

            foreach (var cateId in tagsIds)
            {
                context.Add(new PostTag() { Id = Guid.NewGuid(), PostId = postId, TagId = cateId });
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
                context.Remove(postTag);
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