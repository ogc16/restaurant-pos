using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Cashier")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _service;

    public ReportsController(IReportingService service) => _service = service;

    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyReport([FromQuery] DateTime? date)
    {
        var result = await _service.GetDailyReportAsync(date ?? DateTime.Today);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklyReport([FromQuery] DateTime? startDate)
    {
        var result = await _service.GetWeeklyReportAsync(startDate ?? DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek));
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyReport([FromQuery] int? year, [FromQuery] int? month)
    {
        var now = DateTime.Now;
        var result = await _service.GetMonthlyReportAsync(year ?? now.Year, month ?? now.Month);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportCsv([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _service.ExportSalesToCsvAsync(from, to);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return File(result.Data!, "text/csv", $"sales_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
    }
}
