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
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CategoryName).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<FileUpload>(entity =>
            {
                entity.ToTable("FileUpload");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Path).HasMaxLength(2048);

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.UploadedByNavigation)
                    .WithMany(p => p.FileUploads)
                    .HasForeignKey(d => d.UploadedBy)
                    .HasConstraintName("FK_FileUpload_User");
            });

            modelBuilder.Entity<Otpcode>(entity =>
            {
                entity.ToTable("OTPCode");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Code).HasMaxLength(6);

                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(200);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.AdminStatus).HasMaxLength(10);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.PostName).HasMaxLength(200);

                entity.Property(e => e.Summary).HasMaxLength(200);

                entity.Property(e => e.Thumbnail).HasMaxLength(2048);

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Author)
                    .HasConstraintName("FK_Post_User");
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.ToTable("PostCategory");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.PostCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_PostCategory_Category");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostCategories)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_PostCategory_Post");
            });

            modelBuilder.Entity<PostComment>(entity =>
            {
                entity.ToTable("PostComment");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CommentDate).HasColumnType("datetime");

                entity.Property(e => e.CommenterId).HasColumnName("CommenterID");

                entity.Property(e => e.Content).HasMaxLength(500);

                entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.HasOne(d => d.Commenter)
                    .WithMany(p => p.PostComments)
                    .HasForeignKey(d => d.CommenterId)
                    .HasConstraintName("FK_PostComment_User");

                entity.HasOne(d => d.ParentComment)
                    .WithMany(p => p.InverseParentComment)
                    .HasForeignKey(d => d.ParentCommentId)
                    .HasConstraintName("FK_PostComment_PostComment1");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostComments)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_PostComment_Post");
            });

            modelBuilder.Entity<PostTag>(entity =>
            {
                entity.ToTable("PostTag");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostTags)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_PostTag_Post");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.PostTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_PostTag_Tag");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.TagName).HasMaxLength(200);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Tag_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.PasswordHash).HasMaxLength(2048);

                entity.Property(e => e.PasswordSalt).HasMaxLength(2048);

                entity.Property(e => e.Phone).HasMaxLength(30);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_UserRole");
            });

            modelBuilder.Entity<UserInteractComment>(entity =>
            {
                entity.ToTable("UserInteractComment");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.UserInteractComments)
                    .HasForeignKey(d => d.CommentId)
                    .HasConstraintName("FK_UserInteractComment_PostComment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserInteractComments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserInteractComment_User");
            });

            modelBuilder.Entity<UserInteractPost>(entity =>
            {
                entity.ToTable("UserInteractPost");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Desctiption).HasMaxLength(200);

                entity.Property(e => e.RoleName).HasMaxLength(200);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
