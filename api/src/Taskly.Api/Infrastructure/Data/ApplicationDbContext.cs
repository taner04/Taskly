using Microsoft.EntityFrameworkCore;
using Taskly.Domain.TodoAggregate;

namespace Taskly.Api.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}