using RuleWay.Domain.Entities;

namespace RuleWay.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product> AddAsync(Product product);

        Task UpdateAsync(Product product);

        Task DeleteAsync(Product product);

        Task<List<Product>> FilterAsync(
            string? keyword,
            int? minStock,
            int? maxStock);
    }
}