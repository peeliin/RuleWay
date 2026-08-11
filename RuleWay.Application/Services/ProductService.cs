using RuleWay.Application.Interfaces;
using RuleWay.Domain.Entities;

namespace RuleWay.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await ValidateProductAsync(product);
            return await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            await ValidateProductAsync(product);
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            await _productRepository.DeleteAsync(product);
        }

        public async Task<List<Product>> FilterAsync(
            string? keyword,
            int? minStock,
            int? maxStock)
        {
            if (minStock.HasValue &&
                maxStock.HasValue &&
                minStock.Value > maxStock.Value)
            {
                throw new ArgumentException(
                    "Minimum stock cannot be greater than maximum stock.");
            }

            return await _productRepository.FilterAsync(
                keyword,
                minStock,
                maxStock);
        }

        private async Task ValidateProductAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Title))
                throw new ArgumentException("Title cannot be empty.");

            if (product.Title.Length > 200)
                throw new ArgumentException(
                    "Title cannot be longer than 200 characters.");

            if (!product.CategoryId.HasValue)
            {
                product.IsLive = false;
                return;
            }

            var category = await _categoryRepository
                .GetByIdAsync(product.CategoryId.Value);

            if (category == null)
                throw new ArgumentException("Category not found.");

            product.IsLive =
                product.StockQuantity >= category.MinimumStockQuantity;
        }
    }
}