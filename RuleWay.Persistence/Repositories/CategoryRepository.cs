using Microsoft.EntityFrameworkCore;
using RuleWay.Application.Interfaces;
using RuleWay.Domain.Entities;
using RuleWay.Persistence.Context;

namespace RuleWay.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RuleWayDbContext _context;

        public CategoryRepository(RuleWayDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.Id == id,
                    cancellationToken);
        }

        public async Task<Category> AddAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(
                category,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return category;
        }
    }
}