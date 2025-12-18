using System.Security.Claims;
using AlertrixAPI.Application.DTOs;
using AlertrixAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AlertrixAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _service;
        private readonly ILogger<AlertsController> _logger;

        public AlertsController(IAlertService service, ILogger<AlertsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string GetCurrentUserEmail()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value;
            if (string.IsNullOrEmpty(email)) throw new ApplicationException("User email not found in token.");
            return email.ToLowerInvariant();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var userEmail = GetCurrentUserEmail();
            var items = await _service.GetAlertsAsync(userEmail, page, pageSize, cancellationToken);
            return Ok(items.Items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AlertCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var userEmail = GetCurrentUserEmail();
            var created = await _service.CreateAsync(userEmail, dto, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            var userEmail = GetCurrentUserEmail();
            var removed = await _service.DeleteAsync(id, userEmail, cancellationToken);
            return removed ? NoContent() : NotFound();
        }
    }
}
