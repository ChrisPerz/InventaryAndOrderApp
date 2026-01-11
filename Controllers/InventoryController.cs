using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;

    public InventoryController(LogiTrackContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllInventoryItems([FromQuery]int page = 1, [FromQuery]int pageSize = 5) // Pagination parameters for efficiency improvement
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        // Build a unique cache key per page and pageSize
        var version = _cache.Get<int>("InventoryItems_Version");
        var cacheKey = $"InventoryItems_v{version}_Page{page}_Size{pageSize}";

        if (!_cache.TryGetValue(cacheKey, out List<InventoryItemDto> items))
        {
            items = await _context.InventoryItems
            .AsNoTracking()
            .Select(i => new InventoryItemDto { Name = i.Name, Quantity = i.Quantity, Location = i.Location, Id = i.ItemId })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            _cache.Set(cacheKey, items, TimeSpan.FromMinutes(5));
        
        }
        stopwatch.Stop();
        Console.WriteLine($"GetAllInventoryItems executed in {stopwatch.ElapsedMilliseconds} ms");
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddInventoryItem([FromBody] InventoryItem newItem)
    {
        _context.InventoryItems.Add(newItem);
        await _context.SaveChangesAsync();
        // _cache.Remove("InventoryItems"); // Invalidate cache
        // Increment cache version instead of removing a single key - this is because we have multiple paged cache entries
        var currentVersion = _cache.Get<int>("InventoryItems_Version");
        _cache.Set("InventoryItems_Version", currentVersion + 1);

        return Ok(newItem);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        // Check if the item exists without loading the full entity
        var exists = await _context.InventoryItems.AnyAsync(i => i.ItemId == id);
        if (!exists)
        {
            return NotFound();
        }

        // Create a stub entity with only the ID and mark it for deletion
        _context.InventoryItems.Remove(new InventoryItem { ItemId = id });
        await _context.SaveChangesAsync();

        // Invalidate cache to avoid returning stale data
        _cache.Remove("InventoryItems");

        // Return the deleted item ID as confirmation
        return Ok(new { DeletedItemId = id });
    }
}