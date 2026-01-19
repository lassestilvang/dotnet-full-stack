using InventoryHub.Shared.Models;

namespace InventoryHub.Api.Services;

/// <summary>
/// Interface for inventory business logic.
/// </summary>
public interface IInventoryService
{
    Task<ApiResponse<PaginatedResponse<InventoryItem>>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<InventoryItem>> GetByIdAsync(int id);
    Task<ApiResponse<InventoryItem>> CreateAsync(CreateInventoryItemDto dto);
    Task<ApiResponse<InventoryItem>> UpdateAsync(int id, UpdateInventoryItemDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

/// <summary>
/// Service layer for inventory operations.
/// Implements business logic and uses repository for data access.
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IInventoryRepository repository, ILogger<InventoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiResponse<PaginatedResponse<InventoryItem>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            // Validate pagination parameters
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var items = await _repository.GetPagedAsync(pageNumber, pageSize);
            var totalCount = await _repository.GetTotalCountAsync();

            var paginatedResponse = new PaginatedResponse<InventoryItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {Count} items (Page {Page}/{Total})", 
                items.Count(), pageNumber, paginatedResponse.TotalPages);

            return ApiResponse<PaginatedResponse<InventoryItem>>.SuccessResponse(
                paginatedResponse, 
                $"Retrieved {items.Count()} items successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory items");
            return ApiResponse<PaginatedResponse<InventoryItem>>.ErrorResponse(
                "Failed to retrieve inventory items",
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<InventoryItem>> GetByIdAsync(int id)
    {
        try
        {
            var item = await _repository.GetByIdAsync(id);
            
            if (item == null)
            {
                _logger.LogWarning("Item with ID {Id} not found", id);
                return ApiResponse<InventoryItem>.ErrorResponse(
                    $"Item with ID {id} not found",
                    new List<string> { "The requested item does not exist" });
            }

            return ApiResponse<InventoryItem>.SuccessResponse(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item {Id}", id);
            return ApiResponse<InventoryItem>.ErrorResponse(
                "Failed to retrieve item",
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<InventoryItem>> CreateAsync(CreateInventoryItemDto dto)
    {
        try
        {
            var item = new InventoryItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Category = dto.Category
            };

            var created = await _repository.CreateAsync(item);
            
            _logger.LogInformation("Created new item with ID {Id}: {Name}", created.Id, created.Name);
            
            return ApiResponse<InventoryItem>.SuccessResponse(created, "Item created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item");
            return ApiResponse<InventoryItem>.ErrorResponse(
                "Failed to create item",
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<InventoryItem>> UpdateAsync(int id, UpdateInventoryItemDto dto)
    {
        try
        {
            var item = new InventoryItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Category = dto.Category
            };

            var updated = await _repository.UpdateAsync(id, item);
            
            if (updated == null)
            {
                _logger.LogWarning("Failed to update: Item with ID {Id} not found", id);
                return ApiResponse<InventoryItem>.ErrorResponse(
                    $"Item with ID {id} not found",
                    new List<string> { "The item to update does not exist" });
            }

            _logger.LogInformation("Updated item {Id}: {Name}", id, updated.Name);
            
            return ApiResponse<InventoryItem>.SuccessResponse(updated, "Item updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item {Id}", id);
            return ApiResponse<InventoryItem>.ErrorResponse(
                "Failed to update item",
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var deleted = await _repository.DeleteAsync(id);
            
            if (!deleted)
            {
                _logger.LogWarning("Failed to delete: Item with ID {Id} not found", id);
                return ApiResponse<bool>.ErrorResponse(
                    $"Item with ID {id} not found",
                    new List<string> { "The item to delete does not exist" });
            }

            _logger.LogInformation("Deleted item {Id}", id);
            
            return ApiResponse<bool>.SuccessResponse(true, "Item deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item {Id}", id);
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete item",
                new List<string> { ex.Message });
        }
    }
}
