namespace TuongTLCBE.Data.Entities
{
    public partial class PostTag
    {
        public Guid Id { get; set; }
        public Guid? PostId { get; set; }
        public Guid? TagId { get; set; }

        public virtual Post? Post { get; set; }
        public virtual Tag? Tag { get; set; }
    }
}
