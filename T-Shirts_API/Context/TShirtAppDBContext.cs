using Microsoft.EntityFrameworkCore;
using T_Shirts_API.Data;

namespace T_Shirts_API.Context
{
    public class TShirtAppDBContext : DbContext
    {
        public TShirtAppDBContext(DbContextOptions<TShirtAppDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== العلاقات =====

            // 1:1 بين User و Cart
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            // User -> Orders
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart -> CartItems
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);

            // Order -> OrderItems
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // Product -> CartItems
            modelBuilder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId);

            // Product -> OrderItems (Restrict: مايتمسحش منتج اتطلب قبل كده)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category -> Products
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            

            // ===== القيود (Constraints) =====

            // نفس المنتج مايتكررش في نفس العربية
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();

            // الإيميل مايتكررش
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // ===== دقة الأرقام العشرية =====
            modelBuilder.Entity<Product>()
                .Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount).HasPrecision(18, 2);

            // ===== تخزين الـ enums كنص (اختياري) =====
            modelBuilder.Entity<Order>()
                .Property(o => o.Status).HasConversion<string>();
            modelBuilder.Entity<User>()
                .Property(u => u.Role).HasConversion<string>();
        }
    }
}