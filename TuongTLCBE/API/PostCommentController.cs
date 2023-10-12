using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

[Route("post-comment")]
[ApiController]
public class PostCommentController : ControllerBase
{
    private readonly PostCommentService _postCommentService;

    public PostCommentController(PostCommentService postCommentService)
    {
        _postCommentService = postCommentService;
    }

    [HttpPost("insert-comment")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> InsertComment(PostCommentInsertModel postCommentInsertModel)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postCommentService.InsertPostComment(postCommentInsertModel, token);
        return result.GetType() == typeof(PostCommentModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-post-comments")]
    [AllowAnonymous]
    public async Task<ActionResult> GetPostComment(Guid postId)
    {
        var result = await _postCommentService.GetPostComments(postId);
        return result.GetType() == typeof(List<PostCommentModel>) ? Ok(result) : BadRequest(result);
    }
}