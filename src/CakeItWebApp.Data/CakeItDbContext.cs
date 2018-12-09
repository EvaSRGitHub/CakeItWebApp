using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.Models;
using CakeItWebApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<ShoppingCartItem> CartItems { get; set; }

        public DbSet<Order> Oreders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Ingredients>().Property(e => e.Sponge).HasConversion(v => v.ToString(), v => (SpongeType)Enum.Parse(typeof(SpongeType), v));

            builder.Entity<Ingredients>().Property(e => e.FirstLayerCream).HasConversion(v => v.ToString(), v => (CreamType)Enum.Parse(typeof(CreamType), v));

            builder.Entity<Ingredients>().Property(e => e.SecondLayerCream).HasConversion(v => v.ToString(), v => (CreamType)Enum.Parse(typeof(CreamType), v));

            builder.Entity<Ingredients>().Property(e => e.Filling).HasConversion(v => v.ToString(), v => (FillingType)Enum.Parse(typeof(FillingType), v));

            builder.Entity<Ingredients>().Property(e => e.SideDecoration).HasConversion(v => v.ToString(), v => (SideDecorationType)Enum.Parse(typeof(SideDecorationType), v));

            builder.Entity<Ingredients>().Property(e => e.TopDecoration).HasConversion(v => v.ToString(), v => (TopDecorationType)Enum.Parse(typeof(TopDecorationType), v));

            builder.Entity<Category>().Property(e => e.Type).HasConversion(v => v.ToString(), v => (CategoryType)Enum.Parse(typeof(CategoryType), v));
        }
    }
}
