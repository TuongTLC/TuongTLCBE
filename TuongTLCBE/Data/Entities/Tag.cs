namespace TuongTLCBE.Data.Entities
{
    public partial class Tag
    {
        public Tag()
        {
            PostTags = new HashSet<PostTag>();
        }

        public Guid Id { get; set; }
        public string TagName { get; set; } = null!;
        public string? Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<PostTag> PostTags { get; set; }
    }
}
