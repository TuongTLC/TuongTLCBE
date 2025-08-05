using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class PostCommentService
{
    private readonly DecodeToken _decodeToken;
    private readonly PostCommentRepo _postCommentRepo;
    private readonly UserRepo _userRepo;

    public PostCommentService(PostCommentRepo postCommentRepo, DecodeToken decodeToken, UserRepo userRepo)
    {
        _postCommentRepo = postCommentRepo;
        _decodeToken = decodeToken;
        _userRepo = userRepo;
    }

    private async Task<PostCommentModel?> GetPostComment(Guid postCommentId)
    {
        try
        {
            PostComment? result = await _postCommentRepo.Get(postCommentId);
            if (result != null)
            {
                User? user = await _userRepo.Get(result.CommenterId ?? Guid.Empty);
                if (user != null)
                {
                    PostCommenter postCommenter = new()
                    {
                        Id = user.Id,
                        CommenterName = user.FullName ?? string.Empty,
                        Username = user.Username ?? string.Empty
                    };
                    PostCommentModel responseModel = new()
                    {
                        Id = result.Id,
                        Commenter = postCommenter,
                        PostId = result.PostId ?? Guid.Empty,
                        ParentCommentId = result.ParentCommentId,
                        Content = result.Content ?? string.Empty,
                        CommentDate = result.CommentDate,
                        Like = result.Like,
                        Dislike = result.Dislike,
                        Status = result.Status
                    };
                    return responseModel;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static List<PostCommentModel> BuildCommentTree(List<PostCommentModel> comments, Guid? parentId)
    {
        List<PostCommentModel> commentTree = new();

        foreach (PostCommentModel comment in comments)
        {
            if (comment.ParentCommentId == parentId)
            {
                comment.Replies = BuildCommentTree(comments, comment.Id);
                commentTree.Add(comment);
            }
        }

        return commentTree;
    }

    public async Task<object> GetPostComments(Guid postId)
    {
        try
        {
            List<PostCommentModel>? getPostComments = await _postCommentRepo.GetPostComments(postId);
            return getPostComments != null ? BuildCommentTree(getPostComments, null) : "Something went wrong while retrieving post's comments";
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
            string userId = _decodeToken.Decode(token, "userid");
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
            PostComment? insert = await _postCommentRepo.Insert(postComment);
            if (insert != null)
            {
                PostCommentModel? result = await GetPostComment(insert.Id);
                if (result != null)
                {
                    return result;
                }
            }

            return "Something went wrong while insert comment!";
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> UpdateComment(PostCommentUpdateModel commentUpdateModel, string token)
    {
        try
        {
            string userid = _decodeToken.Decode(token, "userid");
            PostComment? comment = await _postCommentRepo.Get(commentUpdateModel.CommentId);
            if (comment != null && !comment.CommenterId.Equals(Guid.Parse(userid)))
            {
                return "Not the owner of the comment!!!";
            }

            bool updateResult = await _postCommentRepo.UpdateComment(commentUpdateModel);
            return updateResult;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object?> LikeComment(string commentId, string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            return await _postCommentRepo.LikeComment(commentId, userId);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object?> DislikeComment(string commentId, string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            return await _postCommentRepo.DislikeComment(commentId, userId);
        }
        catch (Exception e)
        {
            return e;
        }
    }
}