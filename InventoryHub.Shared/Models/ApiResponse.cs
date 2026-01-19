namespace InventoryHub.Shared.Models;

/// <summary>
/// Standardized API response wrapper for consistent JSON structure.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Errors = new List<string>()
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// Paginated response for list endpoints.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
