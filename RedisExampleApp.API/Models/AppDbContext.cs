using Microsoft.EntityFrameworkCore;

namespace RedisExampleApp.API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // ctor insa ettik cunku addDbContext class'ini new'ledigimizde bu constructor calisacak.
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // manuel data ekleme
            modelBuilder.Entity<Product>().HasData(
                new Product() { ProductId = 1, ProductName = "Product 1", Price = 100 },
                new Product() { ProductId = 2, ProductName = "Product 2", Price = 200 },
                new Product() { ProductId = 3, ProductName = "Product 3", Price = 300 },
                new Product() { ProductId = 4, ProductName = "Product 4", Price = 400 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
