using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Interfaces;

public interface ITableService
{
    Task<Result<List<TableResponse>>> GetAllAsync();
    Task<Result<List<TableResponse>>> GetAvailableAsync();
    Task<Result<TableResponse>> GetByIdAsync(int id);
    Task<Result<TableResponse>> CreateAsync(CreateTableRequest request);
    Task<Result<TableResponse>> UpdateAsync(int id, UpdateTableRequest request);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<PaginatedResponse<TableResponse>>> GetAllPaginatedAsync(PaginatedRequest request);
}
