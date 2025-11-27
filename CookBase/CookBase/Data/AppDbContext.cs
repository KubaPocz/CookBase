using Microsoft.EntityFrameworkCore;
using System;

namespace CookBase.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeProduct> RecipeProducts { get; set; }

        public AppDbContext() : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string databasePath = GetDatabasePath();
            options.UseSqlite($"Filename={databasePath}");
        }

        private string GetDatabasePath()
        {
            string databaseFilename = "cookbase.db";

            // Dla Android - specjalna ścieżka
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return Path.Combine(FileSystem.AppDataDirectory, databaseFilename);
            }
            // Dla Windows/iOS/Mac
            else
            {
                return Path.Combine(FileSystem.AppDataDirectory, databaseFilename);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for RecipeProduct
            modelBuilder.Entity<RecipeProduct>()
                .HasKey(rp => new { rp.RecipeId, rp.ProductId });

            // RecipeProduct → Recipe (many-to-one)
            modelBuilder.Entity<RecipeProduct>()
                .HasOne(rp => rp.Recipe)
                .WithMany(r => r.RecipeProducts)
                .HasForeignKey(rp => rp.RecipeId);

            // RecipeProduct → Product (many-to-one)
            modelBuilder.Entity<RecipeProduct>()
                .HasOne(rp => rp.Product)
                .WithMany(p => p.RecipeProducts)
                .HasForeignKey(rp => rp.ProductId);

            // Category (1..*) Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}