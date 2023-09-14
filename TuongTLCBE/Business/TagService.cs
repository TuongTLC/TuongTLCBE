using Microsoft.IdentityModel.Tokens;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class TagService
{
    private readonly TagRepo _tagRepo;
    private readonly DecodeToken _decodeToken;

    public TagService(TagRepo tagRepo, DecodeToken decodeToken)
    {
        _tagRepo = tagRepo;
        _decodeToken = decodeToken;
    }

    public async Task<object> CreateTag(TagInsertModel tagInsertModel, string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            if (tagInsertModel.TagName.IsNullOrEmpty())
            {
                return "Tag name invalid!";
            }

            Tag insertTag = new()
            {
                Id = Guid.NewGuid(),
                TagName = tagInsertModel.TagName,
                Description = tagInsertModel.Description,
                CreatedBy = Guid.Parse(userId),
                CreatedDate = DateTime.Now
            };
            Tag? insert = await _tagRepo.Insert(insertTag);
            if (insert != null)
            {
                TagModel res = new()
                {
                    Id = insert.Id,
                    TagName = insert.TagName,
                    Description = insert.Description,
                    CreatedBy = insert.CreatedBy,
                    CreatedDate = insert.CreatedDate,
                    Status = insert.Status
                };
                return res;
            }
            else
            {
                return "Create tag failed!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> UpdateTag(TagUpdateModel tagUpdateModel)
    {
        try
        {
            bool update = await _tagRepo.UpdateTag(tagUpdateModel);
            if (update)
            {
                Tag? res = await _tagRepo.Get(tagUpdateModel.Id);
                if (res != null)
                {
                    TagModel resCate = new()
                    {
                        Id = res.Id,
                        TagName = res.TagName,
                        Description = res.Description,
                        CreatedBy = res.CreatedBy,
                        CreatedDate = res.CreatedDate,
                        Status = res.Status
                    };
                    return resCate;
                }
                else
                {
                    return "Failed to retrieve updated tag!";
                }
            }
            else
            {
                return "Failed to update tag!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> ChangeTagStatus(Guid tagId, bool status)
    {
        try
        {
            bool result = await _tagRepo.ChangeTagStatus(tagId, status);
            if (result)
            {
                return true;
            }
            else
            {
                return "Change tag status failed!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> DeleteTag(Guid tagId)
    {
        try
        {
            Tag? getCate = await _tagRepo.Get(tagId);
            if (getCate != null)
            {
                int result = await _tagRepo.Delete(getCate);
                if (result>0)
                {
                    return true;
                }
                else
                {
                    return "Delete tag failed!";
                }
            }
            else
            {
                return "Tag not exist!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> GetATag(Guid tagId)
    {
        try
        {
            TagModel? tagModel = await _tagRepo.GetATag(tagId);
            if (tagModel != null)
            {
                return tagModel;
            }
            else
            {
                return "Tag not found!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public async Task<object> GetTags(string? status)
    {
        try
        {
            List<TagModel>? tagModel = await _tagRepo.GetTags(status);
            if (tagModel != null)
            {
                return tagModel;
            }
            else
            {
                return "Error while getting categories!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }
}