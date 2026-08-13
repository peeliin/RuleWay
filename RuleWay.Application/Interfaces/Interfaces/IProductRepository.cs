using RuleWay.Application.Common;
using RuleWay.Domain.Entities;

namespace RuleWay.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetAllAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Product> AddAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task<PagedResult<Product>> FilterAsync(
            string? keyword,
            int? minStock,
            int? maxStock,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}