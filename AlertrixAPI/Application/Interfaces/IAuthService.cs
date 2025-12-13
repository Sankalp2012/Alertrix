using AlertrixAPI.Application.DTOs;

namespace AlertrixAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string ipAddress, CancellationToken cancellationToken);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, string ipAddress, CancellationToken cancellationToken);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken);
    }
}
