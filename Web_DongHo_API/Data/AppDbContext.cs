using Microsoft.EntityFrameworkCore;

namespace Web_DongHo_API.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillDetails> BillDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleName)
                      .IsRequired()
                      .HasMaxLength(20);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);

                entity.Property(e => e.FullName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.UserName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Password)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.BirthDate)
                      .HasColumnType("date");

                entity.HasOne(e => e.Role)
                      .WithMany(e => e.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(e => e.Bills)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryName)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductImages)
                      .HasColumnType("nvarchar(max)");

                entity.HasOne(e => e.Category)
                      .WithMany(e => e.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.Brand)
                      .WithMany(e => e.Products)
                      .HasForeignKey(e => e.BrandId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.BrandId);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.BillId);

                entity.HasOne(e => e.User)
                      .WithMany(e => e.Bills)
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(e => e.BillDetails)
                      .WithOne(e => e.Bill)
                      .HasForeignKey(e => e.BillId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BillDetails>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Bill)
                      .WithMany(e => e.BillDetails)
                      .HasForeignKey(e => e.BillId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany(e => e.BillDetails)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
