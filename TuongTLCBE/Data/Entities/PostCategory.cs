namespace TuongTLCBE.Data.Entities
{
    public partial class PostCategory
    {
        public Guid Id { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Post? Post { get; set; }
    }
}
