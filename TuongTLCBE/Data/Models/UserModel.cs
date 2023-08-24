namespace TuongTLCBE.Data.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RoleName { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
    }
}

