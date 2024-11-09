using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class PostTagService
{
    private readonly PostTagRepo _postTagRepo;

    public PostTagService(PostTagRepo postTagRepo)
    {
        _postTagRepo = postTagRepo;
    }

    public async Task<object?> GetPostTags(Guid postId)
    {
        try
        {
            return await _postTagRepo.GetPostTags(postId);
        }
        catch (Exception? e)
        {
            return e;
        }
    }
    public async Task<object> InsertPostTag(Guid postId, List<Guid> tagsIds)
    {
        try
        {
            if (tagsIds.Any())
            {
                foreach (Guid cateId in tagsIds)
                {
                    PostTag postTag = new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = postId,
                        TagId = cateId
                    };
                    _ = await _postTagRepo.Insert(postTag);
                }
                return _postTagRepo.GetPostTags(postId);
            }
            else
            {
                return "Tags ID is empty!";
            }

        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> UpdatePostTag(Guid postId, List<Guid> tagsIds)
    {
        try
        {
            if (tagsIds.Any())
            {
                bool result = await _postTagRepo.UpdatePostTags(postId, tagsIds);
                return result ? _postTagRepo.GetPostTags(postId) : "Update post's tags failed!";
            }
            else
            {
                return "No tag id to update!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> DeletePostTag(Guid postId, Guid tagId)
    {
        try
        {
            bool result = await _postTagRepo.DeletePostTag(postId, tagId);
            return result ? _postTagRepo.GetPostTags(postId) : "Delete post tag failed!";
        }
        catch (Exception e)
        {
            return e;
        }
    }
}