using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class Otpcode
{
    public int Code { get; set; }

    public string Email { get; set; } = null!;

    public Guid Id { get; set; }
}