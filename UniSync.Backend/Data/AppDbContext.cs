using Microsoft.EntityFrameworkCore;
using UniSync.Shared.Models;

namespace UniSync.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ScheduleCommit> Commits => Set<ScheduleCommit>();
}