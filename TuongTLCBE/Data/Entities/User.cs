using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid? RoleId { get; set; }

    public string? Phone { get; set; }

    public DateTime? Birthday { get; set; }

    public bool? Status { get; set; }

    public bool? Ban { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<FileUpload> FileUploads { get; set; } = new List<FileUpload>();

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual UserRole? Role { get; set; }

    public virtual ICollection<UserInteractComment> UserInteractComments { get; set; } =
        new List<UserInteractComment>();

    public virtual ICollection<UserInteractPost> UserInteractPosts { get; set; } = new List<UserInteractPost>();
}