using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

[Route("post-tag/")]
[ApiController]
public class PostTagController: ControllerBase
{
    private readonly PostTagService _postTagService;

    public PostTagController(PostTagService postTagService)
    {
        _postTagService = postTagService;
    }

    [HttpGet("get-post-tag")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> GetPostTag(Guid postId)
    {
        object? result = await _postTagService.GetPostTags(postId);
        return (result?.GetType() == typeof(List<PostTag>)) ? Ok(result) : BadRequest(result);
    }
    [HttpPost("add-post-tags")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> AddPostTags(PostTagRequestModel request)
    {
        object result = await _postTagService.InsertPostTag(request.PostId, request.TagsIds);
        return (result.GetType() == typeof(List<PostTag>)) ? Ok(result) : BadRequest(result);
    }
    [HttpPost("update-post-tags")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> UpdatePostTags(PostTagRequestModel request)
    {
        object result = await _postTagService.UpdatePostTag(request.PostId, request.TagsIds);
        return (result.GetType() == typeof(List<PostTag>)) ? Ok(result) : BadRequest(result);
    }
    [HttpDelete("delete-post-tag")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> DeletePostTags(Guid postId, Guid tagId)
    {
        object result = await _postTagService.DeletePostTag(postId, tagId);
        return (result.GetType() == typeof(List<PostTag>)) ? Ok(result) : BadRequest(result);
    }
}