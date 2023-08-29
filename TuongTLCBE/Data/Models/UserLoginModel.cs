using System.ComponentModel.DataAnnotations;

namespace TuongTLCBE.Data.Models
{
    public class UserLoginReqModel
    {
        [Required(ErrorMessage = "Required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }
    }
    public class UserLoginModel
    {
        public string Username { get; set; }
        public string RoleName { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
    }
    public class UserLoginResModel
    {
        public string Token { get; set; }
        public UserLoginModel UserInfo { get; set; }
    }
}

