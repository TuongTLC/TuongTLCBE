using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class UserInteractComment
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? CommentId { get; set; }

    public virtual PostComment? Comment { get; set; }

    public virtual User? User { get; set; }
}