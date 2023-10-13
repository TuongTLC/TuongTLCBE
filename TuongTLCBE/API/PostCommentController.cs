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

    [HttpPost("update-comment")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> UpdateComment(PostCommentUpdateModel commentUpdateModel)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postCommentService.UpdateComment(commentUpdateModel, token);
        if (result is bool)
        {
            if ((bool)result) return Ok("Comment updated!!!");

            if ((bool)result == false) return BadRequest("Comment update failed!!!");
        }

        return BadRequest(result);
    }

    [HttpPost("like-comment")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> LikeComment(string commentId)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postCommentService.LikeComment(commentId, token);
        if (result is bool) return (bool)result ? Ok("Comment liked!!!") : BadRequest("Comment like failed!!!");
        return BadRequest("Already interact!");
    }

    [HttpPost("dislike-comment")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DislikeComment(string commentId)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _postCommentService.DislikeComment(commentId, token);
        if (result is bool) return (bool)result ? Ok("Comment disliked!!!") : BadRequest("Comment dislike failed!!!");
        return BadRequest("Already interact!");
    }
}