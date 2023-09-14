﻿using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid? RoleId { get; set; }

    public string? Phone { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual Post? Post { get; set; }

    public virtual UserRole? Role { get; set; }
}
