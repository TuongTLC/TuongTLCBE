using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

[Route("post")]
[ApiController]
public class PostController : Controller
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpPost("create-post")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> CreatePost(PostRequestModel postRequestModel)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postService.InsertPost(postRequestModel, token);
        return result.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-post")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPost(Guid postId)
    {
        var result = await _postService.GetPost(postId);
        return result?.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-posts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts(int pageNumber, int pageSize, string? status, bool ban, Guid? categoryId,
        Guid? tagId)
    {
        var result = await _postService.GetPosts(pageNumber, pageSize, status, ban, categoryId, tagId);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-user-posts-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserPostsByAdmin(int pageNumber, int pageSize, string userId)
    {
        var result = await _postService.GetUserPostsByAdmin(pageNumber, pageSize, userId);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-user-posts")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetPostsByUser(int pageNumber, int pageSize)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postService.GetPostsByUser(pageNumber, pageSize, token);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-top-liked-posts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopLiked()
    {
        var result = await _postService.GetTopLiked();
        return result.GetType() == typeof(List<PostInfoModel>) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-related-posts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRelatedPosts(string postId)
    {
        var result = await _postService.GetRelatedPosts(postId);
        return Ok(result);
    }

    [HttpGet("search-post")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchPost(int pageNumber, int pageSize, string postName, string? status,
        Guid? categoryId, Guid? tagId)
    {
        var result = await _postService.SearchPosts(pageNumber, pageSize, postName, status, categoryId, tagId);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("update-post")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UpdatePost(PostUpdateModel postUpdateModel)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postService.UpdatePost(postUpdateModel, token);
        return result.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("change-post-status")]
    [SwaggerOperation(Summary = "Active/Inactive")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> ChangePostStatus(Guid postId, string status)
    {
        var result = await _postService.ChangePostStatus(postId, status);
        return result.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("ban-post")]
    [SwaggerOperation(Summary = "Admin ban post")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> BanPost(string postId)
    {
        var result = await _postService.BanPost(postId);
        if ((bool)result) return Ok("Post banned!!!");
        return BadRequest("Post ban failed!!!");
    }

    [HttpPost("unban-post")]
    [SwaggerOperation(Summary = "Admin unban post")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UnbanPost(string postId)
    {
        var result = await _postService.UnbanPost(postId);
        if ((bool)result) return Ok("Post unbanned!!!");
        return BadRequest("Post unban failed!!!");
    }

    [HttpDelete("delete-post")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        var result = await _postService.DeletePost(postId);
        if (result is bool)
            return (bool)result
                ? Ok("Post deleted!")
                : BadRequest("Failed to delete post.");
        return BadRequest(result);
    }

    [HttpPost("like-post")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> LikePost(string postId)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postService.LikePost(postId, token);
        if (result == null) return Conflict("Already interact!");
        if ((bool)result) return Ok("Post liked!!!");
        return BadRequest("Post like failed!!!");
    }

    [HttpPost("dislike-post")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DislikePost(string postId)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postService.DislikePost(postId, token);
        if (result == null) return Conflict("Already interact!");
        if ((bool)result) return Ok("Post disliked!!!");
        return BadRequest("Post dislike failed!!!");
    }
}