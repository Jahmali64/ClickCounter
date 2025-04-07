using ClickCounter.Application.Services.Counter;
using ClickCounter.Application.Services.Counter.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClickCounter.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CounterController : Controller {
    private readonly ICounterService _counterService;
    private readonly ILogger<CounterController> _logger;
    
    public CounterController(ICounterService counterService, ILogger<CounterController> logger) {
        _counterService = counterService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<CounterDto>>> GetCountersAsync() {
        string api = HttpContext.Request.Path.Value ?? string.Empty;
        _logger.LogInformation("Requesting '{api}'", api);

        try {
            List<CounterDto> counterDtos = await _counterService.GetAllAsync();
            _logger.LogInformation("Request to '{api}' processed successfully", api);
            return Ok(counterDtos);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error while processing request to {api}", api);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
