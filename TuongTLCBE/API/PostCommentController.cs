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
    public async Task<ActionResult> InsertComment([FromBody] PostCommentInsertModel postCommentInsertModel)
    {
        string authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }
        string token = authHeader.Split(" ")[1];
        object result = await _postCommentService.InsertPostComment(postCommentInsertModel, token);
        return result.GetType() == typeof(PostCommentModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-post-comments")]
    [AllowAnonymous]
    public async Task<ActionResult> GetPostComment([FromQuery] Guid postId)
    {
        object result = await _postCommentService.GetPostComments(postId);
        return result.GetType() == typeof(List<PostCommentModel>) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("update-comment")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> UpdateComment([FromBody] PostCommentUpdateModel commentUpdateModel)
    {
        string authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }
        string token = authHeader.Split(" ")[1];
        object result = await _postCommentService.UpdateComment(commentUpdateModel, token);
        if (result is bool)
        {
            if ((bool)result)
            {
                return Ok("Comment updated!!!");
            }

            return BadRequest("Comment update failed!!!");
        }

        return BadRequest(result);
    }

    [HttpPost("like-comment")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> LikeComment([FromQuery] string commentId)
    {
        string authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }
        string token = authHeader.Split(" ")[1];
        object? result = await _postCommentService.LikeComment(commentId, token);
        return result == null ? Conflict("Already interact!") : (result is bool && (bool)result) ? Ok("Comment liked!!!") : BadRequest("Comment like failed!!!");
    }

    [HttpPost("dislike-comment")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DislikeComment([FromQuery] string commentId)
    {
        string authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }
        string token = authHeader.Split(" ")[1];
        object? result = await _postCommentService.DislikeComment(commentId, token);
        return result == null
            ? Conflict("Already interact!")
            : (result is bool && (bool)result) ? Ok("Comment disliked!!!") : BadRequest("Comment dislike failed!!!");
    }
}