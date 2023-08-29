﻿namespace TuongTLCBE.Data.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid? RoleId { get; set; }

    public virtual UserRole? Role { get; set; }
}
