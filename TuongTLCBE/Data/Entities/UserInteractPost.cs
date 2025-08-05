namespace TuongTLCBE.Data.Entities
{
    public partial class UserInteractPost
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? PostId { get; set; }
    }
}
