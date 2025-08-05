namespace TuongTLCBE.Data.Entities
{
    public partial class PostComment
    {
        public PostComment()
        {
            InverseParentComment = new HashSet<PostComment>();
            UserInteractComments = new HashSet<UserInteractComment>();
        }

        public Guid Id { get; set; }
        public Guid? CommenterId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string? Content { get; set; }
        public DateTime? CommentDate { get; set; }
        public int? Like { get; set; }
        public int? Dislike { get; set; }
        public bool? Status { get; set; }

        public virtual User? Commenter { get; set; }
        public virtual PostComment? ParentComment { get; set; }
        public virtual Post? Post { get; set; }
        public virtual ICollection<PostComment> InverseParentComment { get; set; }
        public virtual ICollection<UserInteractComment> UserInteractComments { get; set; }
    }
}
