using InventoryHub.Shared.Models;

namespace InventoryHub.Api.Services;

/// <summary>
/// Interface for inventory data operations.
/// </summary>
public interface IInventoryRepository
{
    Task<IEnumerable<InventoryItem>> GetAllAsync();
    Task<InventoryItem?> GetByIdAsync(int id);
    Task<InventoryItem> CreateAsync(InventoryItem item);
    Task<InventoryItem?> UpdateAsync(int id, InventoryItem item);
    Task<bool> DeleteAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<InventoryItem>> GetPagedAsync(int pageNumber, int pageSize);
}

/// <summary>
/// In-memory implementation of the inventory repository.
/// Optimized for performance with thread-safe operations.
/// </summary>
public class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly List<InventoryItem> _items = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private int _nextId = 1;

    public InMemoryInventoryRepository()
    {
        // Seed with sample data
        SeedData();
    }

    private void SeedData()
    {
        var categories = new[] { "Electronics", "Office Supplies", "Furniture", "Software" };
        var random = new Random(42); // Fixed seed for reproducibility

        for (int i = 1; i <= 25; i++)
        {
            _items.Add(new InventoryItem
            {
                Id = _nextId++,
                Name = $"Product {i}",
                Description = $"Description for product {i}. This is a sample inventory item.",
                Quantity = random.Next(1, 100),
                Price = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2),
                Category = categories[random.Next(categories.Length)],
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                UpdatedAt = DateTime.UtcNow
            });
        }
    }

    public async Task<IEnumerable<InventoryItem>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            // Return a copy to prevent external modification
            return _items.Select(i => CloneItem(i)).ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<InventoryItem?> GetByIdAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            return item != null ? CloneItem(item) : null;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<InventoryItem> CreateAsync(InventoryItem item)
    {
        await _lock.WaitAsync();
        try
        {
            item.Id = _nextId++;
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            _items.Add(item);
            return CloneItem(item);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<InventoryItem?> UpdateAsync(int id, InventoryItem item)
    {
        await _lock.WaitAsync();
        try
        {
            var existing = _items.FirstOrDefault(i => i.Id == id);
            if (existing == null) return null;

            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.Quantity = item.Quantity;
            existing.Price = item.Price;
            existing.Category = item.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            return CloneItem(existing);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item == null) return false;
            
            _items.Remove(item);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<int> GetTotalCountAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return _items.Count;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IEnumerable<InventoryItem>> GetPagedAsync(int pageNumber, int pageSize)
    {
        await _lock.WaitAsync();
        try
        {
            return _items
                .OrderByDescending(i => i.UpdatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => CloneItem(i))
                .ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    private static InventoryItem CloneItem(InventoryItem item)
    {
        return new InventoryItem
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Quantity = item.Quantity,
            Price = item.Price,
            Category = item.Category,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
