using Microsoft.AspNetCore.Mvc;

namespace AlertrixAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroqController : ControllerBase
{
    private readonly GroqAlertEvaluator _groq;

    public GroqController(GroqAlertEvaluator groq)
    {
        _groq = groq;
    }

    [HttpGet]
    public async Task<IActionResult> GroqResult()
    {
        var condition = "Has the cricket match restarted after rain?";

        var result = await _groq.IsAlertTriggeredAsync(condition);

        return Ok(new
        {
            condition,
            triggered = result
        });
    }
}
