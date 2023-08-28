using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Expires { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
