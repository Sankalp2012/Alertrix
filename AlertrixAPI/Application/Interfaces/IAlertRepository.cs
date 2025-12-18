using AlertrixAPI.Domain.Entities;

namespace AlertrixAPI.Application.Interfaces
{
    public interface IAlertRepository
    {
        Task<List<Alert>> GetAsync(string userId);
        Task<Alert?> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task CreateAsync(Alert alert, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(string id, string userId, CancellationToken cancellationToken);
        Task<long> CountAsync(string userId);
    }
}
