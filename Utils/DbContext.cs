using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Video> Videos => Set<Video>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
