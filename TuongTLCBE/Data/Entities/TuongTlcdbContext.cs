using Microsoft.EntityFrameworkCore;

namespace TuongTLCBE.Data.Entities;

public partial class TuongTlcdbContext : DbContext
{
    public TuongTlcdbContext(DbContextOptions<TuongTlcdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<FileUpload> FileUploads { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostCategory> PostCategories { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<PostTag> PostTags { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInteractComment?> UserInteractComments { get; set; }

    public virtual DbSet<UserInteractPost> UserInteractPosts { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Category_pk");

            entity.ToTable("Category");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryName).HasMaxLength(200);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Categories)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Category_User_ID_fk");
        });

        modelBuilder.Entity<FileUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("File_pk");

            entity.ToTable("FileUpload");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Path).HasMaxLength(2048);
            entity.Property(e => e.UploadDate).HasColumnType("datetime");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.FileUploads)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("File_User_ID_fk");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Post_pk");

            entity.ToTable("Post");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Dislike).HasDefaultValueSql("((0))");
            entity.Property(e => e.Like).HasDefaultValueSql("((0))");
            entity.Property(e => e.PostName).HasMaxLength(200);
            entity.Property(e => e.Summary).HasMaxLength(200);
            entity.Property(e => e.Thumbnail).HasMaxLength(2048);

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Author)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Post_User_ID_fk");
        });

        modelBuilder.Entity<PostCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PostCategory_pk");

            entity.ToTable("PostCategory");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Category).WithMany(p => p.PostCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostCategory_Category_ID_fk");

            entity.HasOne(d => d.Post).WithMany(p => p.PostCategories)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostCategory_Post_ID_fk");
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PostComment_pk");

            entity.ToTable("PostComment");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CommentDate).HasColumnType("datetime");
            entity.Property(e => e.CommenterId).HasColumnName("CommenterID");
            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.Dislike).HasDefaultValueSql("((0))");
            entity.Property(e => e.Like).HasDefaultValueSql("((0))");
            entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Commenter).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.CommenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostComment_User_ID_fk");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("PostComment_PostComment_ID_fk");

            entity.HasOne(d => d.Post).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostComment_Post_ID_fk");
        });

        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PostTag_pk");

            entity.ToTable("PostTag");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.TagId).HasColumnName("TagID");

            entity.HasOne(d => d.Post).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostTag_Post_ID_fk");

            entity.HasOne(d => d.Tag).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostTag_Tag_ID_fk");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tag_pk");

            entity.ToTable("Tag");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.TagName).HasMaxLength(200);
        });

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
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_BIN");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_UserRole");
        });

        modelBuilder.Entity<UserInteractComment>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("UserInteractComment_pk")
                .IsClustered(false);

            entity.ToTable("UserInteractComment");

            entity.HasIndex(e => new { e.UserId, e.CommentId }, "UserInteractComment_UserID_CommentID_index")
                .IsClustered();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Comment).WithMany(p => p.UserInteractComments)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("UserInteractComment_PostComment_ID_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserInteractComments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserInteractComment_User_ID_fk");
        });

        modelBuilder.Entity<UserInteractPost>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("UserInteractPost_pk")
                .IsClustered(false);

            entity.ToTable("UserInteractPost");

            entity.HasIndex(e => new { e.UserId, e.PostId }, "UserInteractPost_UserID_PostID_index").IsClustered();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.UserInteractPosts)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserInteractPost_Post_ID_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserInteractPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserInteractPost_User_ID_fk");
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