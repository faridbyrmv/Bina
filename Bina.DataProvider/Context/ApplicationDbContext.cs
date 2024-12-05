using Bina.DataProvider.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rooms.Context;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SeedRoles(builder);
    }
    private static void SeedRoles(ModelBuilder model)
    {
        model.Entity<IdentityRole>().HasData(
            new IdentityRole() { Name = "User", ConcurrencyStamp = "1", NormalizedName = "User" },
            new IdentityRole() { Name = "Admin", ConcurrencyStamp = "2", NormalizedName = "Admin" }
            );
    }
    public DbSet<Homes> Home { get; set; }
    public DbSet<Photos> Photos { get; set; }
}