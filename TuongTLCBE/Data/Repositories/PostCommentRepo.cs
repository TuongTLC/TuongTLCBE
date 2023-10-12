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
}