using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class PostCommentRepo : Repository<PostComment>
{
    public PostCommentRepo(TuongTlcdbContext context) : base(context)
    {
    }

    public async Task<List<PostCommentModel>?> GetPostComments(Guid postId)
    {
        try
        {
            var postComments = await context.PostComments.Where(x => x.PostId.Equals(postId)).ToListAsync();
            List<PostCommentModel> postCommentModels = new();
            if (postComments.Any())
                foreach (var postComment in postComments)
                {
                    PostCommentModel convertModel = new()
                    {
                        Id = postComment.Id,
                        CommenterId = postComment.CommenterId,
                        PostId = postComment.PostId,
                        ParentCommentId = postComment.ParentCommentId,
                        Content = postComment.Content,
                        CommentDate = postComment.CommentDate,
                        Like = postComment.Like,
                        Dislike = postComment.Dislike,
                        Status = postComment.Status
                    };
                    postCommentModels.Add(convertModel);
                }

            return postCommentModels;
        }
        catch
        {
            return null;
        }
    }
}