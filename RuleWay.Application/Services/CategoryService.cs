using RuleWay.Application.Interfaces;
using RuleWay.Domain.Entities;

namespace RuleWay.Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            if (category.MinimumStockQuantity < 0)
            {
                throw new ArgumentException("Minimum stock quantity cannot be negative.");
            }

            return await _categoryRepository.AddAsync(category);
        }
    }
}