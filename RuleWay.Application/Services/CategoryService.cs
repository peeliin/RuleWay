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

        public async Task<List<Category>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetAllAsync(
                cancellationToken);
        }

        public async Task<Category?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetByIdAsync(
                id,
                cancellationToken);
        }

        public async Task<Category> CreateAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException(
                    "Category name cannot be empty.");

            if (category.MinimumStockQuantity < 0)
                throw new ArgumentException(
                    "Minimum stock quantity cannot be negative.");

            return await _categoryRepository.AddAsync(
                category,
                cancellationToken);
        }
    }
}