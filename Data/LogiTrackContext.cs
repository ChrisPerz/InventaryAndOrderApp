using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;    
public class LogiTrackContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }

    public LogiTrackContext(DbContextOptions<LogiTrackContext> options) : base(options)
    {
    }

    public LogiTrackContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=logitrack.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship: Order -> InventoryItem
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}