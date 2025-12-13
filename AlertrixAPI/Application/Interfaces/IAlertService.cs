using AlertrixAPI.Application.DTOs;

namespace AlertrixAPI.Application.Interfaces
{
    public interface IAlertService
    {
        Task<(List<AlertDto> Items, long Total)> GetAlertsAsync(string userId, int page, int pageSize, CancellationToken cancellationToken);
        Task<AlertDto?> GetByIdAsync(string id, string userId, CancellationToken cancellationToken);
        Task<AlertDto> CreateAsync(string userId, AlertCreateDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(string userId, string id, CancellationToken cancellationToken);
    }
}
