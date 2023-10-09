using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetPosts(int pageNumber, int pageSize, string? status, Guid? categoryId,
        Guid? tagId)
    {
        var result = await _postService.GetPosts(pageNumber, pageSize, status, categoryId, tagId);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-posts-by-user")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByUser(int pageNumber, int pageSize, string userId)
    {
        var result = await _postService.GetPostsByUser(pageNumber, pageSize, userId);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-top-liked-posts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopLiked()
    {
        var result = await _postService.GetTopLiked();
        return result.GetType() == typeof(List<PostInfoModel>) ? Ok(result) : BadRequest(result);
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangePostStatus(Guid postId, string status)
    {
        var result = await _postService.ChangePostStatus(postId, status);
        return result.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
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
}