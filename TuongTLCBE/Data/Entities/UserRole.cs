using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class UserRole
{
    public Guid Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Desctiption { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
