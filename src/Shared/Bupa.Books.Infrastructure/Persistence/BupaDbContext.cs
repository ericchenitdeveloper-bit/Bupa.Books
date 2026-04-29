using Microsoft.EntityFrameworkCore;

namespace Bupa.Books.Infrastructure.Persistence;

/// <summary>
/// Base DbContext for the Bupa.Books application
/// </summary>
public class BupaDbContext : DbContext
{
    public BupaDbContext(DbContextOptions<BupaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add entity configurations here
    }
}

