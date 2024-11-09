using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuongTLCBE.Business;
using TuongTLCBE.Business.CacheService;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

[Route("post")]
[ApiController]
public class PostController : Controller
{
    private readonly ICacheService _cacheService;
    private readonly PostService _postService;

    public PostController(PostService postService, ICacheService cacheService)
    {
        _postService = postService;
        _cacheService = cacheService;
    }

    [HttpPost("create-post")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> CreatePost(PostRequestModel postRequestModel)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _postService.InsertPost(postRequestModel, token);
        return result.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-post")]
    [SwaggerOperation(Summary = "Get a post info")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPost(Guid postId)
    {
        object? result = await _postService.GetPost(postId);
        return result?.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-posts")]
    [SwaggerOperation(Summary = "Get posts info, status:active/inactive, adminStatus:banned/review/approved")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts(int pageNumber, int pageSize, string? status, string? adminStatus,
        Guid? categoryId,
        Guid? tagId)
    {
        PostPagingResponseModel? cacheData =
            _cacheService.GetData<PostPagingResponseModel>("post" + pageNumber + pageSize + status + adminStatus +
                                                           categoryId +
                                                           tagId);
        if (cacheData != null)
        {
            return Ok(cacheData);
        }

        object result = await _postService.GetPosts(pageNumber, pageSize, status, adminStatus, categoryId, tagId);
        DateTimeOffset expireTime = DateTimeOffset.Now.AddHours(1);
        _ = _cacheService.SetData("post" + pageNumber + pageSize + status + adminStatus + categoryId +
                              tagId, result, expireTime);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-user-posts-admin")]
    [SwaggerOperation(Summary = "Admin get user's posts")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserPostsByAdmin(int pageNumber, int pageSize, string userId)
    {
        object result = await _postService.GetUserPostsByAdmin(pageNumber, pageSize, userId);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-user-posts")]
    [SwaggerOperation(Summary = "User get their own posts")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetPostsByUser(int pageNumber, int pageSize)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _postService.GetPostsByUser(pageNumber, pageSize, token);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-top-liked-posts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopLiked()
    {
        object result = await _postService.GetTopLiked();
        return result.GetType() == typeof(List<PostInfoModel>) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-related-posts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRelatedPosts(string postId)
    {
        object? result = await _postService.GetRelatedPosts(postId);
        return Ok(result);
    }

    [HttpGet("search-post")]
    [SwaggerOperation(Summary = "Search posts, status:active/inactive, adminStatus:banned/review/approved")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchPost(int pageNumber, int pageSize, string postName, string? status,
        string? adminStatus,
        Guid? categoryId, Guid? tagId)
    {
        object result =
            await _postService.SearchPosts(pageNumber, pageSize, postName, status, adminStatus, categoryId, tagId);
        return result.GetType() == typeof(PostPagingResponseModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("update-post")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UpdatePost(PostUpdateModel postUpdateModel)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _postService.UpdatePost(postUpdateModel, token);
        return result.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("change-post-status")]
    [SwaggerOperation(Summary = "Active/Inactive")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> ChangePostStatus(ChangePostStatusModel changePostStatusModel)
    {
        object result = await _postService.ChangePostStatus(changePostStatusModel.PostId, changePostStatusModel.Status);
        return result.GetType() == typeof(PostInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("ban-post")]
    [SwaggerOperation(Summary = "Admin ban post")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> BanPost(string postId)
    {
        object result = await _postService.BanPost(postId);
        return (bool)result ? Ok("Post banned!!!") : BadRequest("Post ban failed!!!");
    }

    [HttpPost("approve-post")]
    [SwaggerOperation(Summary = "Admin approve post")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ApprovePost(string postId)
    {
        object result = await _postService.ApprovePost(postId);
        return (bool)result ? Ok("Post approved!!!") : BadRequest("Post approved failed!!!");
    }

    [HttpPost("unban-post")]
    [SwaggerOperation(Summary = "Admin unban post")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UnbanPost(string postId)
    {
        object result = await _postService.UnbanPost(postId);
        return (bool)result ? Ok("Post unbanned!!!") : BadRequest("Post unban failed!!!");
    }

    [HttpDelete("delete-post")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        object result = await _postService.DeletePost(postId);
        return result is bool
            ? (bool)result
                ? Ok("Post deleted!")
                : BadRequest("Failed to delete post.")
            : BadRequest(result);
    }

    [HttpPost("like-post")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> LikePost(string postId)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object? result = await _postService.LikePost(postId, token);
        return result == null ? Conflict("Already interact!") : (bool)result ? Ok("Post liked!!!") : BadRequest("Post like failed!!!");
    }

    [HttpPost("dislike-post")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DislikePost(string postId)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object? result = await _postService.DislikePost(postId, token);
        return result == null ? Conflict("Already interact!") : (bool)result ? Ok("Post disliked!!!") : BadRequest("Post dislike failed!!!");
    }

    [HttpPost("delete-cache")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCache(string prefix)
    {
        object result = await _cacheService.RemoveOldCache(prefix);
        return result is bool ? Ok(result) : BadRequest(result);
    }
}