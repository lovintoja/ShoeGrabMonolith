using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;

namespace ShoeGrabMonolith.Database.Contexts;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Address { get; set; }

    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
