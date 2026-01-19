using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Shared.Models;

/// <summary>
/// DTO for creating a new inventory item.
/// </summary>
public class CreateInventoryItemDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Category must be between 1 and 50 characters")]
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating an existing inventory item.
/// </summary>
public class UpdateInventoryItemDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Category must be between 1 and 50 characters")]
    public string Category { get; set; } = string.Empty;
}
