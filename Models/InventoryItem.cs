using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class InventoryItem
{
    [Key]
    public int ItemId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int Quantity { get; set; }
    public string Location { get; set; }

    // Foreign key for the one-to-many relationship
    [ForeignKey("Order")]
    public int? OrderId { get; set; }

    // Navigation property: Many InventoryItems belong to one Order
    public Order? Order { get; set; }

    public void DisplayInfo()
    {
        Console.WriteLine($"Item ID: {ItemId}, Name: {Name}, Quantity: {Quantity}, Location: {Location}, Order ID: {OrderId}");
    }
}