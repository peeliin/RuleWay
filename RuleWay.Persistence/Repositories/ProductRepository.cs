using Microsoft.EntityFrameworkCore;
using RuleWay.Application.Common;
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

        public async Task<PagedResult<Product>> GetAllAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category);

            var totalCount = await query.CountAsync(cancellationToken);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(
                    p => p.Id == id,
                    cancellationToken);
        }

        public async Task<Product> AddAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(
                product,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return product;
        }

        public async Task UpdateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResult<Product>> FilterAsync(
            string? keyword,
            int? minStock,
            int? maxStock,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
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

            var totalCount = await query.CountAsync(cancellationToken);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}