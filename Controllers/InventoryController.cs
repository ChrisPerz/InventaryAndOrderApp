using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;

    public InventoryController(LogiTrackContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInventoryItems()
    {
        var items = await _context.InventoryItems.ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddInventoryItem([FromBody] InventoryItem newItem)
    {
        _context.InventoryItems.Add(newItem);
        await _context.SaveChangesAsync();
        return Ok(newItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(item);
    }
}