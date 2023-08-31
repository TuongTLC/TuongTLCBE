using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TuongTLCBE.Data.Entities;

public partial class TuongTlcdbContext : DbContext
{
    public TuongTlcdbContext(DbContextOptions<TuongTlcdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "EmailUnique").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Birthday).HasColumnType("date");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(2048);
            entity.Property(e => e.PasswordSalt).HasMaxLength(2048);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("('38b3d081-a7bc-4ed2-a394-f47d01263e0e')")
                .HasColumnName("RoleID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_BIN");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_UserRole");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC277692154C");

            entity.ToTable("UserRole");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Desctiption).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
