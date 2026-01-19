using System.Net.Http.Json;
using InventoryHub.Shared.Models;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for communicating with the Inventory API.
/// Handles HTTP requests and JSON deserialization.
/// </summary>
public interface IInventoryService
{
    Task<ApiResponse<PaginatedResponse<InventoryItem>>?> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<InventoryItem>?> GetByIdAsync(int id);
    Task<ApiResponse<InventoryItem>?> CreateAsync(CreateInventoryItemDto item);
    Task<ApiResponse<InventoryItem>?> UpdateAsync(int id, UpdateInventoryItemDto item);
    Task<ApiResponse<bool>?> DeleteAsync(int id);
}

public class InventoryService : IInventoryService
{
    private readonly HttpClient _httpClient;

    public InventoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<PaginatedResponse<InventoryItem>>?> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedResponse<InventoryItem>>>(
                $"api/inventory?pageNumber={pageNumber}&pageSize={pageSize}");
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResponse<InventoryItem>>.ErrorResponse(
                "Failed to connect to server", 
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<InventoryItem>?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<InventoryItem>>($"api/inventory/{id}");
        }
        catch (Exception ex)
        {
            return ApiResponse<InventoryItem>.ErrorResponse(
                "Failed to retrieve item", 
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<InventoryItem>?> CreateAsync(CreateInventoryItemDto item)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/inventory", item);
            return await response.Content.ReadFromJsonAsync<ApiResponse<InventoryItem>>();
        }
        catch (Exception ex)
        {
            return ApiResponse<InventoryItem>.ErrorResponse(
                "Failed to create item", 
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<InventoryItem>?> UpdateAsync(int id, UpdateInventoryItemDto item)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/inventory/{id}", item);
            return await response.Content.ReadFromJsonAsync<ApiResponse<InventoryItem>>();
        }
        catch (Exception ex)
        {
            return ApiResponse<InventoryItem>.ErrorResponse(
                "Failed to update item", 
                new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<bool>?> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/inventory/{id}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete item", 
                new List<string> { ex.Message });
        }
    }
}
