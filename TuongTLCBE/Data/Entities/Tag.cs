using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class Tag
{
    public Guid Id { get; set; }

    public string TagName { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}