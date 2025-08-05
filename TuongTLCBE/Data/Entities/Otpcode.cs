namespace TuongTLCBE.Data.Entities
{
    public partial class Otpcode
    {
        public string? Code { get; set; }
        public string? Email { get; set; }
        public Guid Id { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
