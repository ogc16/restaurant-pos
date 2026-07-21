using System.Text;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class ReportingService : IReportingService
{
    private readonly IOrderRepository _orderRepo;

    public ReportingService(IOrderRepository orderRepo) => _orderRepo = orderRepo;

    public async Task<Result<DailySalesReport>> GetDailyReportAsync(DateTime date)
    {
        var from = date.Date;
        var to = from.AddDays(1);
        var orders = await _orderRepo.GetByDateRangeAsync(from, to);
        var completed = orders.Where(o => o.Status == OrderStatus.Completed).ToList();

        var topItems = completed
            .SelectMany(o => o.Items.Where(i => !i.IsVoided))
            .GroupBy(i => new { i.MenuItemId, i.MenuItem?.Name })
            .Select(g => new TopSellingItem(g.Key.MenuItemId, g.Key.Name ?? "", g.Sum(i => i.Quantity), g.Sum(i => i.TotalPrice)))
            .OrderByDescending(t => t.QuantitySold)
            .Take(10)
            .ToList();

        return Result<DailySalesReport>.Success(new DailySalesReport(
            date.Date,
            completed.Count,
            completed.Sum(o => o.SubTotal),
            completed.Sum(o => o.TipAmount),
            completed.SelectMany(o => o.Items).Where(i => !i.IsVoided).Sum(i => i.Quantity),
            topItems));
    }

    public async Task<Result<List<DailySalesReport>>> GetWeeklyReportAsync(DateTime startDate)
    {
        var reports = new List<DailySalesReport>();
        for (int i = 0; i < 7; i++)
        {
            var result = await GetDailyReportAsync(startDate.AddDays(i));
            if (result.IsSuccess) reports.Add(result.Data!);
        }
        return Result<List<DailySalesReport>>.Success(reports);
    }

    public async Task<Result<List<DailySalesReport>>> GetMonthlyReportAsync(int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var reports = new List<DailySalesReport>();
        for (int i = 1; i <= daysInMonth; i++)
        {
            var result = await GetDailyReportAsync(new DateTime(year, month, i));
            if (result.IsSuccess) reports.Add(result.Data!);
        }
        return Result<List<DailySalesReport>>.Success(reports);
    }

    public async Task<Result<byte[]>> ExportSalesToCsvAsync(DateTime from, DateTime to)
    {
        var orders = await _orderRepo.GetByDateRangeAsync(from, to);
        var completed = orders.Where(o => o.Status == OrderStatus.Completed).ToList();

        var sb = new StringBuilder();
        sb.AppendLine("Order ID,Table,Date,Subtotal,Tax,Tip,Total,Payment Method");

        foreach (var o in completed)
        {
            sb.AppendLine($"{o.Id},{o.Table?.TableNumber},{o.CreatedAt:yyyy-MM-dd HH:mm}," +
                $"{o.SubTotal},{o.TaxAmount},{o.TipAmount},{o.TotalAmount}," +
                $"{o.Payment?.Method}");
        }

        return Result<byte[]>.Success(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
