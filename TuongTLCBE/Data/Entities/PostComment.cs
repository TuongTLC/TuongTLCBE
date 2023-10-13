using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class PostComment
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

    public virtual User Commenter { get; set; } = null!;

    public virtual ICollection<PostComment> InverseParentComment { get; set; } = new List<PostComment>();

    public virtual PostComment? ParentComment { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual ICollection<UserInteractComment> UserInteractComments { get; set; } = new List<UserInteractComment>();
}
