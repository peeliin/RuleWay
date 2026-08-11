using Microsoft.EntityFrameworkCore;
using RuleWay.Application.Interfaces;
using RuleWay.Domain.Entities;
using RuleWay.Persistence.Context;

namespace RuleWay.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly RuleWayDbContext _context;

        public ProductRepository(RuleWayDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> FilterAsync(
            string? keyword,
            int? minStock,
            int? maxStock)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Description.Contains(keyword) ||
                    (p.Category != null &&
                     p.Category.Name.Contains(keyword)));
            }

            if (minStock.HasValue)
            {
                query = query.Where(p =>
                    p.StockQuantity >= minStock.Value);
            }

            if (maxStock.HasValue)
            {
                query = query.Where(p =>
                    p.StockQuantity <= maxStock.Value);
            }

            return await query.ToListAsync();
        }
    }
}