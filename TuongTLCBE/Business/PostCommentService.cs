using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class PostCommentService
{
    private readonly DecodeToken _decodeToken;
    private readonly PostCommentRepo _postCommentRepo;

    public PostCommentService(PostCommentRepo postCommentRepo, DecodeToken decodeToken)
    {
        _postCommentRepo = postCommentRepo;
        _decodeToken = decodeToken;
    }

    private async Task<PostCommentModel?> GetPostComment(Guid postCommentId)
    {
        try
        {
            var result = await _postCommentRepo.Get(postCommentId);
            if (result != null)
            {
                PostCommentModel responseModel = new()
                {
                    Id = result.Id,
                    CommenterId = result.CommenterId,
                    PostId = result.PostId,
                    ParentCommentId = result.ParentCommentId,
                    Content = result.Content,
                    CommentDate = result.CommentDate,
                    Like = result.Like,
                    Dislike = result.Dislike,
                    Status = result.Status
                };
                return responseModel;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<object> GetPostComments(Guid postId)
    {
        try
        {
            var getPostComments = await _postCommentRepo.GetPostComments(postId);
            if (getPostComments != null) return getPostComments;

            return "Something went wrong while retrieving post's comments";
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> InsertPostComment(PostCommentInsertModel postCommentInsertModel, string token)
    {
        try
        {
            var userId = _decodeToken.Decode(token, "userid");
            PostComment postComment = new()
            {
                Id = Guid.NewGuid(),
                CommenterId = Guid.Parse(userId),
                PostId = postCommentInsertModel.PostId,
                ParentCommentId = postCommentInsertModel.ParentCommentId,
                Content = postCommentInsertModel.CommentContent,
                CommentDate = DateTime.Now,
                Like = 0,
                Dislike = 0,
                Status = false
            };
            var insert = await _postCommentRepo.Insert(postComment);
            if (insert != null)
            {
                var result = await GetPostComment(insert.Id);
                if (result != null) return result;
            }

            return "Something went wrong while insert comment!";
        }
        catch (Exception e)
        {
            return e;
        }
    }
}