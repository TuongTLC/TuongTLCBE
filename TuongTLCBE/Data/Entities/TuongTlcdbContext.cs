using Microsoft.EntityFrameworkCore;

namespace TuongTLCBE.Data.Entities
{
    public partial class TuongTLCDBContext : DbContext
    {
        public TuongTLCDBContext()
        {
        }

        public TuongTLCDBContext(DbContextOptions<TuongTLCDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<FileUpload> FileUploads { get; set; } = null!;
        public virtual DbSet<Otpcode> Otpcodes { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<PostCategory> PostCategories { get; set; } = null!;
        public virtual DbSet<PostComment> PostComments { get; set; } = null!;
        public virtual DbSet<PostTag> PostTags { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserInteractComment> UserInteractComments { get; set; } = null!;
        public virtual DbSet<UserInteractPost> UserInteractPosts { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Category>(entity =>
            {
                _ = entity.ToTable("Category");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.CategoryName).HasMaxLength(200);

                _ = entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                _ = entity.Property(e => e.Description).HasMaxLength(500);

                _ = entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                _ = entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Category_User_ID_fk");
            });

            _ = modelBuilder.Entity<FileUpload>(entity =>
            {
                _ = entity.ToTable("FileUpload");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.Path).HasMaxLength(2048);

                _ = entity.Property(e => e.UploadDate).HasColumnType("datetime");

                _ = entity.HasOne(d => d.UploadedByNavigation)
                    .WithMany(p => p.FileUploads)
                    .HasForeignKey(d => d.UploadedBy)
                    .HasConstraintName("File_User_ID_fk");
            });

            _ = modelBuilder.Entity<Otpcode>(entity =>
            {
                _ = entity.ToTable("OTPCode");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.Code).HasMaxLength(6);

                _ = entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                _ = entity.Property(e => e.Email).HasMaxLength(200);
            });

            _ = modelBuilder.Entity<Post>(entity =>
            {
                _ = entity.ToTable("Post");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.AdminStatus)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("('review')");

                _ = entity.Property(e => e.CreateDate).HasColumnType("datetime");

                _ = entity.Property(e => e.Dislike).HasDefaultValueSql("((0))");

                _ = entity.Property(e => e.Like).HasDefaultValueSql("((0))");

                _ = entity.Property(e => e.PostName).HasMaxLength(200);

                _ = entity.Property(e => e.Summary).HasMaxLength(200);

                _ = entity.Property(e => e.Thumbnail).HasMaxLength(2048);

                _ = entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Author)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Post_User_ID_fk");
            });

            _ = modelBuilder.Entity<PostCategory>(entity =>
            {
                _ = entity.ToTable("PostCategory");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                _ = entity.Property(e => e.PostId).HasColumnName("PostID");

                _ = entity.HasOne(d => d.Category)
                    .WithMany(p => p.PostCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PostCategory_Category_ID_fk");

                _ = entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostCategories)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PostCategory_Post_ID_fk");
            });

            _ = modelBuilder.Entity<PostComment>(entity =>
            {
                _ = entity.ToTable("PostComment");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.CommentDate).HasColumnType("datetime");

                _ = entity.Property(e => e.CommenterId).HasColumnName("CommenterID");

                _ = entity.Property(e => e.Content).HasMaxLength(500);

                _ = entity.Property(e => e.Dislike).HasDefaultValueSql("((0))");

                _ = entity.Property(e => e.Like).HasDefaultValueSql("((0))");

                _ = entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");

                _ = entity.Property(e => e.PostId).HasColumnName("PostID");

                _ = entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                _ = entity.HasOne(d => d.Commenter)
                    .WithMany(p => p.PostComments)
                    .HasForeignKey(d => d.CommenterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PostComment_User_ID_fk");

                _ = entity.HasOne(d => d.ParentComment)
                    .WithMany(p => p.InverseParentComment)
                    .HasForeignKey(d => d.ParentCommentId)
                    .HasConstraintName("PostComment_PostComment_ID_fk");

                _ = entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostComments)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PostComment_Post_ID_fk");
            });

            _ = modelBuilder.Entity<PostTag>(entity =>
            {
                _ = entity.ToTable("PostTag");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.PostId).HasColumnName("PostID");

                _ = entity.Property(e => e.TagId).HasColumnName("TagID");

                _ = entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostTags)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PostTag_Post_ID_fk");

                _ = entity.HasOne(d => d.Tag)
                    .WithMany(p => p.PostTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PostTag_Tag_ID_fk");
            });

            _ = modelBuilder.Entity<Tag>(entity =>
            {
                _ = entity.ToTable("Tag");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                _ = entity.Property(e => e.Description).HasMaxLength(500);

                _ = entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                _ = entity.Property(e => e.TagName).HasMaxLength(200);
            });

            _ = modelBuilder.Entity<User>(entity =>
            {
                _ = entity.ToTable("User");

                _ = entity.HasIndex(e => e.Email, "EmailUnique")
                    .IsUnique();

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.Ban).HasDefaultValueSql("((0))");

                _ = entity.Property(e => e.Birthday).HasColumnType("date");

                _ = entity.Property(e => e.Email).HasMaxLength(200);

                _ = entity.Property(e => e.FullName).HasMaxLength(200);

                _ = entity.Property(e => e.PasswordHash).HasMaxLength(2048);

                _ = entity.Property(e => e.PasswordSalt).HasMaxLength(2048);

                _ = entity.Property(e => e.Phone).HasMaxLength(30);

                _ = entity.Property(e => e.RoleId)
                    .HasColumnName("RoleID")
                    .HasDefaultValueSql("('38b3d081-a7bc-4ed2-a394-f47d01263e0e')");

                _ = entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .UseCollation("Latin1_General_BIN");

                _ = entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_UserRole");
            });

            _ = modelBuilder.Entity<UserInteractComment>(entity =>
            {
                _ = entity.HasKey(e => e.Id)
                    .HasName("UserInteractComment_pk")
                    .IsClustered(false);

                _ = entity.ToTable("UserInteractComment");

                _ = entity.HasIndex(e => new { e.UserId, e.CommentId }, "UserInteractComment_UserID_CommentID_index")
                    .IsClustered();

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.CommentId).HasColumnName("CommentID");

                _ = entity.Property(e => e.UserId).HasColumnName("UserID");

                _ = entity.HasOne(d => d.Comment)
                    .WithMany(p => p.UserInteractComments)
                    .HasForeignKey(d => d.CommentId)
                    .HasConstraintName("UserInteractComment_PostComment_ID_fk");

                _ = entity.HasOne(d => d.User)
                    .WithMany(p => p.UserInteractComments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UserInteractComment_User_ID_fk");
            });

            _ = modelBuilder.Entity<UserInteractPost>(entity =>
            {
                _ = entity.HasKey(e => e.Id)
                    .HasName("UserInteractPost_pk")
                    .IsClustered(false);

                _ = entity.ToTable("UserInteractPost");

                _ = entity.HasIndex(e => new { e.UserId, e.PostId }, "UserInteractPost_UserID_PostID_index")
                    .IsClustered();

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.PostId).HasColumnName("PostID");

                _ = entity.Property(e => e.UserId).HasColumnName("UserID");

                _ = entity.HasOne(d => d.Post)
                    .WithMany(p => p.UserInteractPosts)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserInteractPost_Post_ID_fk");

                _ = entity.HasOne(d => d.User)
                    .WithMany(p => p.UserInteractPosts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserInteractPost_User_ID_fk");
            });

            _ = modelBuilder.Entity<UserRole>(entity =>
            {
                _ = entity.ToTable("UserRole");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                _ = entity.Property(e => e.Desctiption).HasMaxLength(200);

                _ = entity.Property(e => e.RoleName).HasMaxLength(200);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
