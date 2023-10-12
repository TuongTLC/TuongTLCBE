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

    public Guid CommenterId { get; set; }

    public Guid PostId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CommentDate { get; set; }

    public int? Like { get; set; }

    public int? Dislike { get; set; }

    public bool? Status { get; set; }
}