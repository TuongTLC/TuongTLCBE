using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class TagRepo : Repository<Tag>
{
    public TagRepo(TuongTlcdbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateTag(TagUpdateModel tagUpdateModel)
    {
        try
        {
            var tag = await context.Tags.Where(x => x.Id.Equals(tagUpdateModel.Id))
                .FirstOrDefaultAsync();
            if (tag != null)
            {
                tag.TagName = tagUpdateModel.TagName;
                tag.Description = tagUpdateModel.Description;
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

    public async Task<bool> ChangeTagStatus(Guid tagId, bool status)
    {
        try
        {
            var tag = await context.Tags.Where(x => x.Id.Equals(tagId))
                .FirstOrDefaultAsync();
            if (tag != null)
            {
                tag.Status = status;
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

    public async Task<TagModel?> GetATag(Guid tagId)
    {
        try
        {
            var query = from c in context.Tags
                where c.Id.Equals(tagId)
                select new { c };
            var tagModel = await query.Select(x => new TagModel
            {
                Id = x.c.Id,
                TagName = x.c.TagName,
                Description = x.c.Description,
                CreatedBy = x.c.CreatedBy,
                CreatedDate = x.c.CreatedDate,
                Status = x.c.Status
            }).FirstOrDefaultAsync();
            return tagModel;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<TagModel>?> GetTags(string? status)
    {
        try
        {
            if (!status.IsNullOrEmpty())
            {
                if (status != null && status.Trim().ToLower().Equals("all"))
                {
                    var query = from c in context.Tags
                        select new { c };
                    var tagModel = await query.Select(x => new TagModel
                    {
                        Id = x.c.Id,
                        TagName = x.c.TagName,
                        Description = x.c.Description,
                        CreatedBy = x.c.CreatedBy,
                        CreatedDate = x.c.CreatedDate,
                        Status = x.c.Status
                    }).ToListAsync();
                    return tagModel;
                }
                else
                {
                    var statusIn = true;
                    if (status != null && status.Trim().ToLower().Equals("active")) statusIn = true;
                    if (status != null && status.Trim().ToLower().Equals("inactive")) statusIn = false;
                    var query = from c in context.Tags
                        where c.Status.Equals(statusIn)
                        select new { c };
                    var tagModel = await query.Select(x => new TagModel
                    {
                        Id = x.c.Id,
                        TagName = x.c.TagName,
                        Description = x.c.Description,
                        CreatedBy = x.c.CreatedBy,
                        CreatedDate = x.c.CreatedDate,
                        Status = x.c.Status
                    }).ToListAsync();
                    return tagModel;
                }
            }

            {
                var query = from c in context.Tags
                    select new { c };
                var tagModel = await query.Select(x => new TagModel
                {
                    Id = x.c.Id,
                    TagName = x.c.TagName,
                    Description = x.c.Description,
                    CreatedBy = x.c.CreatedBy,
                    CreatedDate = x.c.CreatedDate,
                    Status = x.c.Status
                }).ToListAsync();
                return tagModel;
            }
        }
        catch
        {
            return null;
        }
    }
}