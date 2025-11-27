using Microsoft.EntityFrameworkCore;

namespace CookBase.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeProduct> RecipeProducts { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=app.db");
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
