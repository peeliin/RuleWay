using Microsoft.EntityFrameworkCore;
using RuleWay.Domain.Entities;

namespace RuleWay.Persistence.Context
{
    public class RuleWayDbContext : DbContext
    {
        public RuleWayDbContext(DbContextOptions<RuleWayDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}