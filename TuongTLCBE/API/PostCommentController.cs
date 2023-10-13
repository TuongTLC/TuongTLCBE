using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.API;

[Route("post-comment")]
[ApiController]
public class PostCommentController : ControllerBase
{
    private readonly PostCommentRepo _postCommentRepo;
    private readonly PostCommentService _postCommentService;

    public PostCommentController(PostCommentService postCommentService, PostCommentRepo postCommentRepo)
    {
        _postCommentService = postCommentService;
        _postCommentRepo = postCommentRepo;
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
    [AllowAnonymous]
    public async Task<ActionResult> LikeComment(string commentId)
    {
        var result = await _postCommentRepo.LikeComment(commentId);
        if (result) return Ok("Comment liked!!!");
        return BadRequest("Comment like failed!!!");
    }

    [HttpPost("dislike-comment")]
    [AllowAnonymous]
    public async Task<ActionResult> DislikeComment(string commentId)
    {
        var result = await _postCommentRepo.DislikeComment(commentId);
        if (result) return Ok("Comment disliked!!!");
        return BadRequest("Comment dislike failed!!!");
    }
}