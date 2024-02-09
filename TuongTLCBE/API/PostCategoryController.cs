using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

[Route("post-category/")]
[ApiController]
public class PostCategoryController : ControllerBase
{
    private readonly PostCategoryService _postCategoryService;

    public PostCategoryController(PostCategoryService postCategoryService)
    {
        _postCategoryService = postCategoryService;
    }

    [HttpGet("get-post-category")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> GetPostCategory(Guid postId)
    {
        var result = await _postCategoryService.GetPostCategories(postId);
        return result?.GetType() == typeof(List<PostCategory>) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("add-post-categories")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> AddPostCategories(PostCategoryRequestModel request)
    {
        var result = await _postCategoryService.InsertPostCategory(request.PostId, request.CategoriesIds);
        return result.GetType() == typeof(List<PostCategory>) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("update-post-categories")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> UpdatePostCategories(PostCategoryRequestModel request)
    {
        var result = await _postCategoryService.UpdatePostCategory(request.PostId, request.CategoriesIds);
        return result.GetType() == typeof(List<PostCategory>) ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("delete-post-category")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> DeletePostCategories(Guid postId, Guid categoryId)
    {
        var result = await _postCategoryService.DeletePostCategory(postId, categoryId);
        return result.GetType() == typeof(List<PostCategory>) ? Ok(result) : BadRequest(result);
    }
}