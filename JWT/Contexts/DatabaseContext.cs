using JWT.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT.Contexts;

public class DatabaseContext : DbContext
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<LogToken> RefreshTokens { get; set; }

    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}