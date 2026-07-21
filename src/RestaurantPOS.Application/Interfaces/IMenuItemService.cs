using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Interfaces;

public interface IMenuItemService
{
    Task<Result<List<MenuItemResponse>>> GetAllAsync();
    Task<Result<List<MenuItemResponse>>> GetByCategoryAsync(string category);
    Task<Result<List<MenuItemResponse>>> GetDailySpecialsAsync();
    Task<Result<MenuItemResponse>> GetByIdAsync(int id);
    Task<Result<MenuItemResponse>> CreateAsync(CreateMenuItemRequest request);
    Task<Result<MenuItemResponse>> UpdateAsync(int id, UpdateMenuItemRequest request);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> ToggleAvailabilityAsync(int id);
}
