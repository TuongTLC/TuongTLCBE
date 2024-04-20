using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities
{
    public partial class User
    {
        public User()
        {
            Categories = new HashSet<Category>();
            FileUploads = new HashSet<FileUpload>();
            PostComments = new HashSet<PostComment>();
            Posts = new HashSet<Post>();
            UserInteractComments = new HashSet<UserInteractComment>();
            UserInteractPosts = new HashSet<UserInteractPost>();
        }

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

        public virtual UserRole? Role { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<FileUpload> FileUploads { get; set; }
        public virtual ICollection<PostComment> PostComments { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<UserInteractComment> UserInteractComments { get; set; }
        public virtual ICollection<UserInteractPost> UserInteractPosts { get; set; }
    }
}
