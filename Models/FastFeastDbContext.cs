namespace _2001230507_NhanTuManh_B2.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FastFeastDbContext : DbContext
    {
        public FastFeastDbContext()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<DeliveryDriver> DeliveryDrivers { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ProductOption> ProductOptions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.TotalAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.DeliveryFee)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.DiscountAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Deliveries)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductOption>()
                .Property(e => e.AdditionalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ProductOption>()
                .HasMany(e => e.OrderDetails)
                .WithMany(e => e.ProductOptions)
                .Map(m => m.ToTable("OrderDetail_Options").MapLeftKey("OptionID").MapRightKey("OrderDetailID"));

            modelBuilder.Entity<ProductOption>()
                .HasMany(e => e.Products)
                .WithMany(e => e.ProductOptions)
                .Map(m => m.ToTable("Product_ProductOptions").MapLeftKey("OptionID").MapRightKey("ProductID"));

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Promotion>()
                .Property(e => e.DiscountValue)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Promotion>()
                .Property(e => e.MinimumOrderAmount)
                .HasPrecision(10, 2);
        }
    }
}
