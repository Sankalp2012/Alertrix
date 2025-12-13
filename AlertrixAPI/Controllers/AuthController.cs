using AlertrixAPI.Application.DTOs;
using AlertrixAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlertrixAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var resp = await _auth.RegisterAsync(dto, ip, cancellationToken);
            return Created("", resp);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var resp = await _auth.LoginAsync(dto, ip, cancellationToken);
            return Ok(resp);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken, CancellationToken cancellationToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var resp = await _auth.RefreshTokenAsync(refreshToken, ip, cancellationToken);
            return Ok(resp);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] string refreshToken, CancellationToken cancellationToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _auth.RevokeRefreshTokenAsync(refreshToken, ip, cancellationToken);
            return result ? NoContent() : NotFound();
        }
    }
}
