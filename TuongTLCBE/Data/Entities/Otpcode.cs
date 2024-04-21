using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities
{
    public partial class Otpcode
    {
        public string Code { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Guid Id { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
