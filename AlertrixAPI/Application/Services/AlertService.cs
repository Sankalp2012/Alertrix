using AlertrixAPI.Application.DTOs;
using AlertrixAPI.Application.Interfaces;
using AlertrixAPI.Domain.Entities;

namespace AlertrixAPI.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _repo;

        public AlertService(IAlertRepository repo)
        {
            _repo = repo;
        }

        public async Task<(List<AlertDto> Items, long Total)> GetAlertsAsync(string userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var items = await _repo.GetAsync(userId, page, pageSize, cancellationToken);
            var total = await _repo.CountAsync(userId, cancellationToken);
            var dtos = items.Select(MapToDto).ToList();
            return (dtos, total);
        }

        public async Task<AlertDto?> GetByIdAsync(string id, string userId, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByIdAsync(id, cancellationToken);
            if (entity == null || entity.UserId != userId) return null;
            return MapToDto(entity);
        }

        public async Task<AlertDto> CreateAsync(string userId, AlertCreateDto dto, CancellationToken cancellationToken)
        {
            var entity = new Alert
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repo.CreateAsync(entity, cancellationToken);
            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(string id, string userId, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAsync(id, userId, cancellationToken);
        }

        private static AlertDto MapToDto(Alert e) => new AlertDto
        {
            Id = e.Id,
            UserId = e.UserId,
            Title = e.Title,
            Description = e.Description,
            CreatedAt = e.CreatedAt,
            IsActive = e.IsActive
        };
    }
}
