using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LogiTrackContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar el servicio OrderPrinter
builder.Services.AddScoped<OrderPrinter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => {
        // Crear un pedido
        var order = new Order
        {
            OrderId = 1001,
            CustomerName = "Samir",
            DatePlaced = DateTime.Now
        };

        // Agregar ítems
        var item1 = new InventoryItem { ItemId = 1, Name = "Laptop", Quantity = 1, Location = "Warehouse" };
        var item2 = new InventoryItem { ItemId = 2, Name = "Mouse", Quantity = 5, Location = "Warehouse" };
        order.AddItem(item1);
        order.AddItem(item2);

        // Quitar un ítem
        order.RemoveItem(item2);
        // Mostrar resumen
        return order.GetOrderSummary();
    });

app.MapGet("/inventory", (LogiTrackContext context) =>
{
    // Add test inventory item if none exist
    if (!context.InventoryItems.Any())
    {
        context.InventoryItems.Add(new InventoryItem
        {
            Name = "Pallet Jack",
            Quantity = 12,
            Location = "Warehouse A"
        });

        context.SaveChanges();
    }

    // Retrieve and print inventory to confirm
    var items = context.InventoryItems.ToList();
    foreach (var item in items)
    {
        item.DisplayInfo(); 
    }
    return "Items displayed in console.";
}); 

app.MapGet("/orders/print", (OrderPrinter orderPrinter) =>
{
    return orderPrinter.PrintAllOrdersToString();
});

app.MapGet("/getOrderItems", (LogiTrackContext context) =>
{
    // Add test order and inventory items if none exist
    if (!context.Orders.Any())
    {
        var order = new Order
        {
            CustomerName = "Test Customer",
            DatePlaced = DateTime.Now
        };

        var item1 = new InventoryItem
        {
            Name = "Pallet Jack",
            Quantity = 12,
            Location = "Warehouse A",
            Order = order
        };
        var item2 = new InventoryItem
        {
            Name = "Forklift",
            Quantity = 5,
            Location = "Warehouse B",
            Order = order
        };

        order.Items.Add(item1);
        order.Items.Add(item2);

        context.Orders.Add(order);
        context.SaveChanges();
    }

    // Retrieve and print inventory to confirm
    var items = context.InventoryItems.Include(i => i.Order).ToList();
    foreach (var item in items)
    {
        item.DisplayInfo();
    }
    return "Items displayed in console.";
}); 

app.Run();
