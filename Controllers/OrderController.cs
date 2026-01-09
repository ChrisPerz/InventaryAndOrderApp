using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly LogiTrackContext _context;

    public OrderController(LogiTrackContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Items) // Eager load related items
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
            .ToListAsync();
        if (!orders.Any())
        {
            return NotFound();
        }
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