using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

[Route("category/")]
[ApiController]
public class CategoryController : Controller
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost("create-category")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateCategory(CategoryInsertModel request)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _categoryService.CreateCategory(request, token);
        return (result.GetType() == typeof(CategoryModel)) ? Ok(result) : BadRequest(result);
    }
    [HttpPost("update-category")]
    [SwaggerOperation(Summary = "Admin update category name/description")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateCategory(CategoryUpdateModel request)
    {
        object result = await _categoryService.UpdateCategory(request);
        return (result.GetType() == typeof(CategoryModel)) ? Ok(result) : BadRequest(result);
    }
    [HttpPost("change-category-status")]
    [SwaggerOperation(Summary = "Admin update category status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ChangeCategoryStatus(Guid categoryId, bool status)
    {
        object result = await _categoryService.ChangeCategoryStatus(categoryId, status);
        return (result is bool) ? Ok("Change category status successful!") : BadRequest(result);
    }
    [HttpGet("get-a-category")]
    [AllowAnonymous]
    public async Task<ActionResult> GetACategory(Guid categoryId)
    {
        object result = await _categoryService.GetACategory(categoryId);
        return (result.GetType()==typeof(CategoryModel)) ? Ok(result) : BadRequest(result);
    }
    [HttpGet("get-categories")]
    [SwaggerOperation(Summary = "Get categories by status: active/inactive/all")]

    [AllowAnonymous]
    public async Task<ActionResult> GateCategories(string? status)
    {
        object result = await _categoryService.GetCategories(status);
        return (result.GetType()==typeof(List<CategoryModel>)) ? Ok(result) : BadRequest(result);
    }
    [HttpDelete("delete-category")]
    [SwaggerOperation(Summary = "Admin delete category status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(Guid categoryId)
    {
        object result = await _categoryService.DeleteCategory(categoryId);
        return (result is bool) ? Ok("Delete category successful!") : BadRequest(result);
    }
}