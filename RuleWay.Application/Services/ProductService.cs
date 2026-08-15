using RuleWay.Application.Common;
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

        public async Task<PagedResult<Product>> GetAllAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            return await _productRepository.GetAllAsync(
                page,
                pageSize,
                cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetByIdAsync(
                id,
                cancellationToken);
        }

        public async Task<Product> CreateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            await ValidateProductAsync(product, cancellationToken);

            return await _productRepository.AddAsync(
                product,
                cancellationToken);
        }

        public async Task UpdateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            await ValidateProductAsync(product, cancellationToken);

            await _productRepository.UpdateAsync(
                product,
                cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (product == null)
                throw new KeyNotFoundException("Ürün bulunamadı.");

            await _productRepository.DeleteAsync(
                product,
                cancellationToken);
        }

        public async Task<PagedResult<Product>> FilterAsync(
            string? keyword,
            int? minStock,
            int? maxStock,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (minStock.HasValue &&
                maxStock.HasValue &&
                minStock.Value > maxStock.Value)
            {
                throw new ArgumentException(
                    "Minimum stok adedi maksimum stok adedinden büyük olamaz.");
            }

            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            return await _productRepository.FilterAsync(
                keyword,
                minStock,
                maxStock,
                page,
                pageSize,
                cancellationToken);
        }

        private async Task ValidateProductAsync(
            Product product,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(product.Title))
                throw new ArgumentException("Ürün başlığı boş olamaz.");

            if (product.Title.Length > 200)
                throw new ArgumentException(
                    "Ürün başlığı 200 karakterden uzun olamaz.");

            if (!product.CategoryId.HasValue)
            {
                product.IsLive = false;
                return;
            }

            var category = await _categoryRepository.GetByIdAsync(
                product.CategoryId.Value,
                cancellationToken);

            if (category == null)
                throw new ArgumentException("Kategori bulunamadı.");

            product.IsLive =
                product.StockQuantity >= category.MinimumStockQuantity;
        }
    }
}