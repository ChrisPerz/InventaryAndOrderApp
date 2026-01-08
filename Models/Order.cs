using System.ComponentModel.DataAnnotations;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    [Required]
    public string CustomerName { get; set; }
    public DateTime DatePlaced { get; set; }

    // One-to-many relationship: One Order has many InventoryItems
    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();

    public void AddItem(InventoryItem item)
    {
        Items.Add(item);
    }
    public void RemoveItem(InventoryItem item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
        }
        else
        {
            Console.WriteLine("Item not found in the order.");
        }
    }
    public string GetOrderSummary()
    {
        var itemNames = string.Join(", ", Items.Select(i => i.Name));
        return $"Order ID: {OrderId}, Customer: {CustomerName}, Date: {DatePlaced}, Items: {itemNames}";
    }
}