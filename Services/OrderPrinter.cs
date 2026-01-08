using System.Text;
using Microsoft.EntityFrameworkCore;

public class OrderPrinter
{
    private readonly LogiTrackContext _context;

    public OrderPrinter(LogiTrackContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all order summaries efficiently (avoids N+1 queries)
    /// </summary>
    public List<string> GetAllOrderSummaries()
    {
        return _context.Orders
            .Include(o => o.Items)  // Eager load related items
            .AsNoTracking()         // No tracking for read-only queries
            .Select(o => $"Order ID: {o.OrderId}, Customer: {o.CustomerName}, " +
                         $"Date: {o.DatePlaced}, Items: {string.Join(", ", o.Items.Select(i => i.Name))}")
            .ToList();
    }

    /// <summary>
    /// Prints all orders with formatted separators
    /// </summary>
    public void PrintAllOrders()
    {
        var summaries = GetAllOrderSummaries();
        
        if (summaries.Count == 0)
        {
            Console.WriteLine("No orders available.");
            return;
        }

        Console.WriteLine("=== ORDER SUMMARIES ===\n");
        foreach (var summary in summaries)
        {
            Console.WriteLine(summary);
            Console.WriteLine("---");
        }
    }

    /// <summary>
    /// Gets all summaries as a single string
    /// </summary>
    public string PrintAllOrdersToString()
    {
        var summaries = GetAllOrderSummaries();
        
        if (summaries.Count == 0)
            return "No orders available.";

        var sb = new StringBuilder();
        sb.AppendLine("=== ORDER SUMMARIES ===");
        sb.AppendLine();

        foreach (var summary in summaries)
        {
            sb.AppendLine(summary);
            sb.AppendLine("---");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Prints a specific order
    /// </summary>
    public void PrintOrder(Order order)
    {
        Console.WriteLine(order.GetOrderSummary());
    }
}
