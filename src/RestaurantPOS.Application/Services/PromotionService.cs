using Microsoft.Extensions.Logging;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _repository;
    private readonly ILogger<PromotionService> _logger;

    public PromotionService(IPromotionRepository repository, ILogger<PromotionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<PromotionResponse>>> GetAllAsync()
    {
        var promos = await _repository.GetAllAsync();
        return Result<List<PromotionResponse>>.Success(promos.Select(MapToResponse).ToList());
    }

    public async Task<Result<PromotionResponse>> GetByIdAsync(int id)
    {
        var promo = await _repository.GetByIdAsync(id);
        if (promo == null) return Result<PromotionResponse>.Failure("Promotion not found");
        return Result<PromotionResponse>.Success(MapToResponse(promo));
    }

    public async Task<Result<PromotionResponse>> CreateAsync(CreatePromotionRequest request)
    {
        _logger.LogInformation("Creating Promotion: {Code}", request.Code);
        var existing = await _repository.GetByCodeAsync(request.Code);
        if (existing != null) return Result<PromotionResponse>.Failure("Promotion code already exists");

        var promotion = new Promotion
        {
            Code = request.Code,
            Description = request.Description,
            DiscountPercent = request.DiscountPercent,
            DiscountAmount = request.DiscountAmount,
            IsPercentBased = request.IsPercentBased,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            MaxUses = request.MaxUses
        };
        var created = await _repository.CreateAsync(promotion);
        return Result<PromotionResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<PromotionResponse>> UpdateAsync(int id, UpdatePromotionRequest request)
    {
        _logger.LogInformation("Updating Promotion {Id}", id);
        var promo = await _repository.GetByIdAsync(id);
        if (promo == null) return Result<PromotionResponse>.Failure("Promotion not found");

        if (request.Code != null) promo.Code = request.Code;
        if (request.Description != null) promo.Description = request.Description;
        if (request.DiscountPercent.HasValue) promo.DiscountPercent = request.DiscountPercent.Value;
        if (request.DiscountAmount.HasValue) promo.DiscountAmount = request.DiscountAmount.Value;
        if (request.IsPercentBased.HasValue) promo.IsPercentBased = request.IsPercentBased.Value;
        if (request.ValidFrom.HasValue) promo.ValidFrom = request.ValidFrom.Value;
        if (request.ValidUntil.HasValue) promo.ValidUntil = request.ValidUntil.Value;
        if (request.MaxUses.HasValue) promo.MaxUses = request.MaxUses.Value;

        await _repository.UpdateAsync(promo);
        return Result<PromotionResponse>.Success(MapToResponse(promo));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting Promotion {Id}", id);
        var promo = await _repository.GetByIdAsync(id);
        if (promo == null) return Result<bool>.Failure("Promotion not found");
        await _repository.DeleteAsync(id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PromotionResponse>> ValidateCodeAsync(string code)
    {
        var promo = await _repository.GetByCodeAsync(code);
        if (promo == null) return Result<PromotionResponse>.Failure("Promotion not found");
        if (!promo.IsActive) return Result<PromotionResponse>.Failure("Promotion is expired or not yet active");
        if (promo.MaxUses.HasValue && promo.CurrentUses >= promo.MaxUses.Value)
            return Result<PromotionResponse>.Failure("Promotion usage limit reached");
        return Result<PromotionResponse>.Success(MapToResponse(promo));
    }

    public async Task<Result<PaginatedResponse<PromotionResponse>>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _repository.GetAllPaginatedAsync(request);
        return Result<PaginatedResponse<PromotionResponse>>.Success(new PaginatedResponse<PromotionResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    private static PromotionResponse MapToResponse(Promotion p) =>
        new(p.Id, p.Code, p.Description, p.DiscountPercent, p.DiscountAmount,
            p.IsPercentBased, p.ValidFrom, p.ValidUntil, p.IsActive, p.MaxUses, p.CurrentUses);
}
