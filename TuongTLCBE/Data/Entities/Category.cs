namespace TuongTLCBE.Data.Entities
{
    public partial class Category
    {
        public Category()
        {
            PostCategories = new HashSet<PostCategory>();
        }

        public Guid Id { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<PostCategory> PostCategories { get; set; }
    }
}
