using CakeItWebApp.Models;
using CakeItWebApp.Models.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace CakeItWebApp.Data
{
    public class CakeItDbContext : IdentityDbContext<CakeItUser>
    {
        public CakeItDbContext(DbContextOptions<CakeItDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Tutorial> Tutorials { get; set; }

        public DbSet<Ingredients> Ingredients { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<CustomCakeImg> CustomCakesImg { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<TagPosts> TagPosts { get; set; }

        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Ingredients>().Property(e => e.Sponge).HasConversion(v => v.ToString(), v => (SpongeType)Enum.Parse(typeof(SpongeType), v));

            builder.Entity<Ingredients>().Property(e => e.FirstLayerCream).HasConversion(v => v.ToString(), v => (CreamType)Enum.Parse(typeof(CreamType), v));

            builder.Entity<Ingredients>().Property(e => e.SecondLayerCream).HasConversion(v => v.ToString(), v => (CreamType)Enum.Parse(typeof(CreamType), v));

            builder.Entity<Ingredients>().Property(e => e.Filling).HasConversion(v => v.ToString(), v => (FillingType)Enum.Parse(typeof(FillingType), v));

            builder.Entity<Ingredients>().Property(e => e.SideDecoration).HasConversion(v => v.ToString(), v => (SideDecorationType)Enum.Parse(typeof(SideDecorationType), v));

            builder.Entity<Ingredients>().Property(e => e.TopDecoration).HasConversion(v => v.ToString(), v => (TopDecorationType)Enum.Parse(typeof(TopDecorationType), v));

            builder.Entity<Category>().Property(e => e.Type).HasConversion(v => v.ToString(), v => (CategoryType)Enum.Parse(typeof(CategoryType), v));

            builder.Entity<Order>(entity =>
            {
                entity.HasOne(e => e.OrderDetails).WithOne(od => od.Order).HasForeignKey<OrderDetails>(b => b.OrderId);
            });

            builder.Entity<OrderProduct>(entity => {

                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.HasOne(e => e.Order).WithMany(o => o.Products).HasForeignKey(e => e.OrderId);

                entity.HasOne(e => e.Product).WithMany(p => p.Orders).HasForeignKey(e => e.ProductId);
            });

            builder.Entity<TagPosts>(entity => {
                entity.HasKey(e => new { e.TagId, e.PostId });

                entity.HasOne(e => e.Tag).WithMany(t => t.Posts).HasForeignKey(e => e.TagId);

                entity.HasOne(e => e.Post).WithMany(p => p.Tags).HasForeignKey(e => e.PostId);
            });
        }
    }
}
