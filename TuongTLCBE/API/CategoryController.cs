using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [HttpGet("list-categories")]
    [AllowAnonymous]
    public async Task<ActionResult> ListCategories()
    {
        object result = await _categoryService.GetCategories();
        return (result.GetType() == typeof(List<CategoryModel>)) ? Ok(result) : BadRequest(result);
    }
    [HttpPost("update-category")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateCategory(CategoryUpdateModel request)
    {
        object result = await _categoryService.UpdateCategory(request);
        return (result.GetType() == typeof(CategoryModel)) ? Ok(result) : BadRequest(result);
    }
}