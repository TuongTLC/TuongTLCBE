using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

[Route("tag/")]
[ApiController]
public class TagController : Controller
{
    private readonly TagService _tagService;

    public TagController(TagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost("create-tag")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateTag(TagInsertModel request)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _tagService.CreateTag(request, token);
        return result.GetType() == typeof(TagModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("update-tag")]
    [SwaggerOperation(Summary = "Admin update tag name/description")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateTag(TagUpdateModel request)
    {
        var result = await _tagService.UpdateTag(request);
        return result.GetType() == typeof(TagModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("change-tag-status")]
    [SwaggerOperation(Summary = "Admin update tag status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ChangeTagStatus(Guid tagId, bool status)
    {
        var result = await _tagService.ChangeTagStatus(tagId, status);
        return result is bool ? Ok("Change tag status successful!") : BadRequest(result);
    }

    [HttpGet("get-a-tag")]
    [AllowAnonymous]
    public async Task<ActionResult> GetATag(Guid tagId)
    {
        var result = await _tagService.GetATag(tagId);
        return result.GetType() == typeof(TagModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-tags")]
    [SwaggerOperation(Summary = "Get tags by status: active/inactive/all")]
    [AllowAnonymous]
    public async Task<ActionResult> GateTags(string? status)
    {
        var result = await _tagService.GetTags(status);
        return result.GetType() == typeof(List<TagModel>) ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("delete-tag")]
    [SwaggerOperation(Summary = "Admin delete tag status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTag(Guid tagId)
    {
        var result = await _tagService.DeleteTag(tagId);
        return result is bool ? Ok("Delete tag successful!") : BadRequest(result);
    }
}