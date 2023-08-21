
using Microsoft.EntityFrameworkCore;

namespace TuongTLCBE.Data.Entities;

public partial class TuongTlcdbContext : DbContext
{
    public TuongTlcdbContext(DbContextOptions<TuongTlcdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<RefreshToken>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC2717B9365F");

            _ = entity.ToTable("RefreshToken");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Created).HasColumnType("datetime");
            _ = entity.Property(e => e.Expires).HasColumnType("datetime");
            _ = entity.Property(e => e.Token).HasMaxLength(200);
            _ = entity.Property(e => e.UserId).HasColumnName("UserID");

            _ = entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserID_RefreshToken");
        });

        _ = modelBuilder.Entity<User>(entity =>
        {
            _ = entity.ToTable("User");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Email).HasMaxLength(200);
            _ = entity.Property(e => e.FullName).HasMaxLength(200);
            _ = entity.Property(e => e.PasswordHash).HasMaxLength(2048);
            _ = entity.Property(e => e.PasswordSalt).HasMaxLength(2048);
            _ = entity.Property(e => e.RoleId).HasColumnName("RoleID");
            _ = entity.Property(e => e.Username).HasMaxLength(50);

            _ = entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserRole");
        });

        _ = modelBuilder.Entity<UserRole>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC277692154C");

            _ = entity.ToTable("UserRole");

            _ = entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            _ = entity.Property(e => e.Desctiption).HasMaxLength(200);
            _ = entity.Property(e => e.RoleName).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
