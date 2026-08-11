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

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }
    }
}