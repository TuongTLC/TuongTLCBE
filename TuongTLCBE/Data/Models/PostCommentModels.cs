namespace TuongTLCBE.Data.Models;

public class PostCommentInsertModel
{
    public Guid PostId { get; set; }
    public string CommentContent { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public class PostCommentModel
{
    public Guid Id { get; set; }

    public PostCommenter Commenter { get; set; }

    public Guid PostId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CommentDate { get; set; }

    public int? Like { get; set; }

    public int? Dislike { get; set; }

    public bool? Status { get; set; }

    public List<PostCommentModel> Replies { get; set; } = new();
}

public class PostCommenter
{
    public Guid Id { get; set; }
    public string CommenterName { get; set; }
    public string Username { get; set; }
}

public class PostCommentUpdateModel
{
    public Guid CommentId { get; set; }
    public string? Content { get; set; }
    public bool? Like { get; set; }
    public bool? Dislike { get; set; }
    public bool? Status { get; set; }
}