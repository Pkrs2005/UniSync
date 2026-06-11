using Microsoft.EntityFrameworkCore;
using UniSync.Shared.Models;
using UniSync.Backend.Models; // 👈 Шаг 1: Добавляем using, где лежит класс User

namespace UniSync.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ScheduleCommit> Commits => Set<ScheduleCommit>();
    
    // 👈 Шаг 2: Добавляем таблицу пользователей (в том же стиле Expression-bodied)
    public DbSet<User> Users => Set<User>(); 
}