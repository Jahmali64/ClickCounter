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
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CounterDto>> GetCounterByIdAsync(int id) {
        string api = HttpContext.Request.Path.Value ?? string.Empty;
        _logger.LogInformation("Requesting '{api}'", api);
        
        if (id < 1) {
            _logger.LogWarning("Invalid counter id '{id}'", id);
            return BadRequest("Invalid counter id");
        }

        try {
            CounterDto? counterDto = await _counterService.GetByIdAsync(id);
            if (counterDto is null) {
                _logger.LogWarning("Counter with id '{id}' not found", id);
                return NotFound();
            }
            _logger.LogInformation("Request to '{api}' processed successfully", api);
            return Ok(counterDto);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error while processing request to {api}", api);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<CounterDto>> AddCounterAsync([FromBody] SaveCounterDto saveCounterDto) {
        string api = HttpContext.Request.Path.Value ?? string.Empty;
        _logger.LogInformation("Requesting '{api}'", api);

        try {
            CounterDto counterDto = await _counterService.AddAsync(saveCounterDto);
            _logger.LogInformation("Request to '{api}' processed successfully", api);
            return Ok(counterDto);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error while processing request to {api}", api);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCounterAsync(int id, [FromBody] SaveCounterDto saveCounterDto) {
        string api = HttpContext.Request.Path.Value ?? string.Empty;
        _logger.LogInformation("Requesting '{api}'", api);
        
        if (id < 1) {
            _logger.LogWarning("Invalid counter id '{id}'", id);
            return BadRequest("Invalid counter id");
        }
        
        try {
            int rowsAffected = await _counterService.UpdateAsync(id, saveCounterDto);
            if (rowsAffected == 0) {
                _logger.LogWarning("Counter with id '{id}' not found", id);
                return NotFound();
            }
            _logger.LogInformation("Request to '{api}' processed successfully", api);
            return NoContent();
        } catch (Exception ex) {
            _logger.LogError(ex, "Error while processing request to {api}", api);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCounterAsync(int id) {
        string api = HttpContext.Request.Path.Value ?? string.Empty;
        _logger.LogInformation("Requesting '{api}'", api);
        
        if (id < 1) {
            _logger.LogWarning("Invalid counter id '{id}'", id);
            return BadRequest("Invalid counter id");
        }

        try {
            int rowsAffected = await _counterService.DeleteAsync(id);
            if (rowsAffected == 0) {
                _logger.LogWarning("Counter with id '{id}' not found", id);
                return NotFound();
            }
            _logger.LogInformation("Request to '{api}' processed successfully", api);
            return NoContent();
        } catch (Exception ex) {
            _logger.LogError(ex, "Error while processing request to {api}", api);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
