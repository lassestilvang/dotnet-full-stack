using Microsoft.AspNetCore.Mvc;
using InventoryHub.Api.Services;
using InventoryHub.Shared.Models;

namespace InventoryHub.Api.Controllers;

/// <summary>
/// API Controller for inventory management operations.
/// Provides CRUD endpoints with structured JSON responses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Get all inventory items with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <returns>Paginated list of inventory items</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<InventoryItem>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<InventoryItem>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("GET /api/inventory - Page: {Page}, Size: {Size}", pageNumber, pageSize);
        
        var response = await _inventoryService.GetAllAsync(pageNumber, pageSize);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific inventory item by ID.
    /// </summary>
    /// <param name="id">The item ID</param>
    /// <returns>The inventory item</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InventoryItem>>> GetById(int id)
    {
        _logger.LogInformation("GET /api/inventory/{Id}", id);
        
        var response = await _inventoryService.GetByIdAsync(id);
        
        if (!response.Success)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }

    /// <summary>
    /// Create a new inventory item.
    /// </summary>
    /// <param name="dto">The item creation data</param>
    /// <returns>The created inventory item</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<InventoryItem>>> Create([FromBody] CreateInventoryItemDto dto)
    {
        _logger.LogInformation("POST /api/inventory - Creating: {Name}", dto.Name);
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(ApiResponse<InventoryItem>.ErrorResponse("Validation failed", errors));
        }

        var response = await _inventoryService.CreateAsync(dto);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }
        
        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    /// <summary>
    /// Update an existing inventory item.
    /// </summary>
    /// <param name="id">The item ID to update</param>
    /// <param name="dto">The item update data</param>
    /// <returns>The updated inventory item</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<InventoryItem>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InventoryItem>>> Update(int id, [FromBody] UpdateInventoryItemDto dto)
    {
        _logger.LogInformation("PUT /api/inventory/{Id} - Updating: {Name}", id, dto.Name);
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(ApiResponse<InventoryItem>.ErrorResponse("Validation failed", errors));
        }

        var response = await _inventoryService.UpdateAsync(id, dto);
        
        if (!response.Success)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }

    /// <summary>
    /// Delete an inventory item.
    /// </summary>
    /// <param name="id">The item ID to delete</param>
    /// <returns>Success indicator</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        _logger.LogInformation("DELETE /api/inventory/{Id}", id);
        
        var response = await _inventoryService.DeleteAsync(id);
        
        if (!response.Success)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
}
