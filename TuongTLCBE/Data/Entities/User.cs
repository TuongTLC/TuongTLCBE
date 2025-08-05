namespace TuongTLCBE.Data.Entities
{
    public partial class User
    {
        public User()
        {
            FileUploads = new HashSet<FileUpload>();
            PostComments = new HashSet<PostComment>();
            Posts = new HashSet<Post>();
            Tags = new HashSet<Tag>();
            UserInteractComments = new HashSet<UserInteractComment>();
        }

        public Guid Id { get; set; }
        public string? Username { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public Guid? RoleId { get; set; }
        public string? Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public bool? Status { get; set; }
        public bool? Ban { get; set; }

        public virtual UserRole? Role { get; set; }
        public virtual ICollection<FileUpload> FileUploads { get; set; }
        public virtual ICollection<PostComment> PostComments { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<UserInteractComment> UserInteractComments { get; set; }
    }
}
