using Microsoft.Extensions.Logging;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository _repository;
    private readonly ILogger<MenuItemService> _logger;

    public MenuItemService(IMenuItemRepository repository, ILogger<MenuItemService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<MenuItemResponse>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return Result<List<MenuItemResponse>>.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<MenuItemResponse>>> GetByCategoryAsync(string category)
    {
        var items = await _repository.GetByCategoryAsync(category);
        return Result<List<MenuItemResponse>>.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<MenuItemResponse>>> GetDailySpecialsAsync()
    {
        var items = await _repository.GetDailySpecialsAsync();
        return Result<List<MenuItemResponse>>.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<MenuItemResponse>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return Result<MenuItemResponse>.Failure("Menu item not found");
        return Result<MenuItemResponse>.Success(MapToResponse(item));
    }

    public async Task<Result<MenuItemResponse>> CreateAsync(CreateMenuItemRequest request)
    {
        _logger.LogInformation("Creating MenuItem: {Name}", request.Name);
        var item = new MenuItem
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            ImageUrl = request.ImageUrl,
            PrepTimeMinutes = request.PrepTimeMinutes,
            IsDailySpecial = request.IsDailySpecial
        };

        var created = await _repository.CreateAsync(item);
        return Result<MenuItemResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<MenuItemResponse>> UpdateAsync(int id, UpdateMenuItemRequest request)
    {
        _logger.LogInformation("Updating MenuItem {Id}", id);
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return Result<MenuItemResponse>.Failure("Menu item not found");

        if (request.Name != null) item.Name = request.Name;
        if (request.Description != null) item.Description = request.Description;
        if (request.Price.HasValue) item.Price = request.Price.Value;
        if (request.Category.HasValue) item.Category = request.Category.Value;
        if (request.ImageUrl != null) item.ImageUrl = request.ImageUrl;
        if (request.PrepTimeMinutes.HasValue) item.PrepTimeMinutes = request.PrepTimeMinutes.Value;
        if (request.IsAvailable.HasValue) item.IsAvailable = request.IsAvailable.Value;
        if (request.IsDailySpecial.HasValue) item.IsDailySpecial = request.IsDailySpecial.Value;

        await _repository.UpdateAsync(item);
        return Result<MenuItemResponse>.Success(MapToResponse(item));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting MenuItem {Id}", id);
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return Result<bool>.Failure("Menu item not found");
        await _repository.DeleteAsync(id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ToggleAvailabilityAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return Result<bool>.Failure("Menu item not found");
        item.IsAvailable = !item.IsAvailable;
        await _repository.UpdateAsync(item);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PaginatedResponse<MenuItemResponse>>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _repository.GetAllPaginatedAsync(request);
        return Result<PaginatedResponse<MenuItemResponse>>.Success(new PaginatedResponse<MenuItemResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    private static MenuItemResponse MapToResponse(MenuItem item) =>
        new(item.Id, item.Name, item.Description, item.Price, item.ImageUrl,
            item.Category, item.IsAvailable, item.IsDailySpecial, item.PrepTimeMinutes);
}
