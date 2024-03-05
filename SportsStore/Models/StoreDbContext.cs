using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models{
    public class StoreDbContext: DbContext{
        public StoreDbContext(DbContextOptions<StoreDbContext>options) : base(options)
        { }

        // DbSet<Product> represents the collection of products in the database.
        public DbSet<Product> Products=> Set<Product>();

        // DbSet<Order> represents the collection of orders in the database.
        public DbSet<Order> Orders => Set<Order>();
    }
}
