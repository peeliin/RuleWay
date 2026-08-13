using RuleWay.Domain.Entities;

namespace RuleWay.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<Category?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Category> AddAsync(
            Category category,
            CancellationToken cancellationToken = default);
    }
}