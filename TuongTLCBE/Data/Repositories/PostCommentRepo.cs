using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class PostCommentRepo : Repository<PostComment>
{
    private readonly UserRepo _userRepo;

    public PostCommentRepo(TuongTlcdbContext context, UserRepo userRepo) : base(context)
    {
        _userRepo = userRepo;
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
                    var user = await _userRepo.Get(postComment.CommenterId);
                    if (user != null)
                    {
                        PostCommenter postCommenter = new()
                        {
                            Id = user.Id,
                            CommenterName = user.FullName,
                            Username = user.Username
                        };
                        PostCommentModel convertModel = new()
                        {
                            Id = postComment.Id,
                            Commenter = postCommenter,
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
                }

            return postCommentModels;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<bool> UpdateComment(PostCommentUpdateModel commentUpdateModel)
    {
        try
        {
            var comment = await context.PostComments.Where(x => x.Id.Equals(commentUpdateModel.CommentId))
                .FirstOrDefaultAsync();
            if (comment != null)
            {
                if (commentUpdateModel.Content != null) comment.Content = commentUpdateModel.Content;
                if (commentUpdateModel.Like == true) comment.Like += 1;
                if (commentUpdateModel.Dislike == true) comment.Dislike += 1;
                if (commentUpdateModel.Status != null) comment.Status = commentUpdateModel.Status;
                var update = await context.SaveChangesAsync();
                if (update == 0) return false;
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}