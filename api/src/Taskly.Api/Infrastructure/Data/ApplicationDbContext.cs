using Microsoft.EntityFrameworkCore;
using Taskly.Domain.TaskItems;

namespace Taskly.Api.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem>  TaskItems =>  Set<TaskItem>();
}