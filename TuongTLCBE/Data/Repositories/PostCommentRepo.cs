using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class PostCommentRepo : Repository<PostComment>
{
    private readonly UserInteractCommentRepo _userInteractCommentRepo;
    private readonly UserRepo _userRepo;

    public PostCommentRepo(TuongTLCDBContext context, UserRepo userRepo,
        UserInteractCommentRepo userInteractCommentRepo) : base(context)
    {
        _userRepo = userRepo;
        _userInteractCommentRepo = userInteractCommentRepo;
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

    public async Task<bool?> LikeComment(string commentId, string userId)
    {
        try
        {
            var checkInteraction = await context.UserInteractComments
                .Where(x => x != null && x.UserId.Equals(Guid.Parse(userId)) &&
                            x.CommentId.Equals(Guid.Parse(commentId)))
                .FirstOrDefaultAsync();
            if (checkInteraction != null) return null;
            var comment = await context.PostComments.Where(x => x.Id.Equals(Guid.Parse(commentId)))
                .FirstOrDefaultAsync();
            if (comment != null)
            {
                comment.Like += 1;
                var update = await context.SaveChangesAsync();
                if (update > 0)
                    _ = await _userInteractCommentRepo.Insert(new UserInteractComment
                        { Id = Guid.NewGuid(), UserId = Guid.Parse(userId), CommentId = Guid.Parse(commentId) });
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

    public async Task<bool?> DislikeComment(string commentId, string userId)
    {
        try
        {
            var checkInteraction = await context.UserInteractComments
                .Where(x => x != null && x.UserId.Equals(Guid.Parse(userId)) &&
                            x.CommentId.Equals(Guid.Parse(commentId)))
                .FirstOrDefaultAsync();
            if (checkInteraction != null) return null;
            var comment = await context.PostComments.Where(x => x.Id.Equals(Guid.Parse(commentId)))
                .FirstOrDefaultAsync();
            if (comment != null)
            {
                comment.Dislike += 1;
                var update = await context.SaveChangesAsync();
                if (update > 0)
                    _ = await _userInteractCommentRepo.Insert(new UserInteractComment
                        { Id = Guid.NewGuid(), UserId = Guid.Parse(userId), CommentId = Guid.Parse(commentId) });
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