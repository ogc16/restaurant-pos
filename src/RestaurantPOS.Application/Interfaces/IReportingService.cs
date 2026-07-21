using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Interfaces;

public interface IReportingService
{
    Task<Result<DailySalesReport>> GetDailyReportAsync(DateTime date);
    Task<Result<List<DailySalesReport>>> GetWeeklyReportAsync(DateTime startDate);
    Task<Result<List<DailySalesReport>>> GetMonthlyReportAsync(int year, int month);
    Task<Result<byte[]>> ExportSalesToCsvAsync(DateTime from, DateTime to);
}
