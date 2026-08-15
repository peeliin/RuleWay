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
                    "Kategori adı boş olamaz.");

            if (category.MinimumStockQuantity < 0)
                throw new ArgumentException(
                    "Minimum stok adedi negatif olamaz.");

            return await _categoryRepository.AddAsync(
                category,
                cancellationToken);
        }

        public async Task UpdateAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException(
                    "Kategori adı boş olamaz.");

            if (category.MinimumStockQuantity < 0)
                throw new ArgumentException(
                    "Minimum stok adedi negatif olamaz.");

            await _categoryRepository.UpdateAsync(
                category,
                cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (category == null)
                throw new KeyNotFoundException("Kategori bulunamadı.");

            var hasProducts = await _categoryRepository.HasProductsAsync(
                id,
                cancellationToken);

            if (hasProducts)
                throw new InvalidOperationException(
                    "Bu kategori silinemez çünkü kategoriye bağlı ürünler var. Önce ürünleri silin veya başka bir kategoriye taşıyın.");

            await _categoryRepository.DeleteAsync(
                category,
                cancellationToken);
        }
    }
}