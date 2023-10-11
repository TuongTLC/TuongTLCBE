using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class Post
{
    public Guid Id { get; set; }

    public string PostName { get; set; } = null!;

    public string? Summary { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid Author { get; set; }

    public int? Like { get; set; }

    public int? Dislike { get; set; }

    public string? Thumbnail { get; set; }

    public bool Status { get; set; }

    public string Content { get; set; } = null!;

    public virtual User AuthorNavigation { get; set; } = null!;

    public virtual ICollection<PostCategory> PostCategories { get; set; } = new List<PostCategory>();

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
