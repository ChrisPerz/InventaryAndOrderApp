using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;

    public OrderController(LogiTrackContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }
    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetAllOrders() // here we could also implement pagination if needed
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        if(_cache.TryGetValue("orders", out List<OrderDto> cachedOrders))
        {
            stopwatch.Stop();
            Console.WriteLine($"GetAllOrders executed in {stopwatch.ElapsedMilliseconds} ms");
            return Ok(cachedOrders);
        }
        var orders = await _context.Orders
            .Include(o => o.Items) // Eager load related items and make one query with JOIN instead of N+1
            .Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                DatePlaced = o.DatePlaced,
                Items = o.Items.Select(i => new InventoryItemDto    //<--- Necessary DTO to avoid circular reference
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Location = i.Location
                }).ToList()
            })
            .AsNoTracking() // Improve performance for read-only operations
            .ToListAsync();
        if (!orders.Any())
        {
            stopwatch.Stop();
            Console.WriteLine($"GetAllOrders executed in {stopwatch.ElapsedMilliseconds} ms");
            return NotFound();
        }
        _cache.Set("orders", orders, TimeSpan.FromMinutes(5));
        stopwatch.Stop();
        Console.WriteLine($"GetAllOrders executed in {stopwatch.ElapsedMilliseconds} ms");
        return Ok(orders);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items) 
            .Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                DatePlaced = o.DatePlaced,
                Items = o.Items.Select(i => new InventoryItemDto    //<--- Necessary DTO to avoid circular reference
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Location = i.Location
                }).ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order newOrder)
    {
        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();
        _cache.Remove("orders"); // Invalidate cache
        var dto = new OrderDto
        {
            OrderId = newOrder.OrderId,
            CustomerName = newOrder.CustomerName,
            DatePlaced = newOrder.DatePlaced,
            Items = newOrder.Items.Select(i => new InventoryItemDto    //<--- Necessary DTO to avoid circular reference
            {
                Name = i.Name,
                Quantity = i.Quantity,
                Location = i.Location
            }).ToList()
        };
        return Ok(dto);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        var dto = new OrderDto
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            DatePlaced = order.DatePlaced,
            Items = order.Items.Select(i => new InventoryItemDto    //<--- Necessary DTO to avoid circular reference
            {
                Name = i.Name,
                Quantity = i.Quantity,
                Location = i.Location
            }).ToList()
        };

        return Ok(dto);
    }
}