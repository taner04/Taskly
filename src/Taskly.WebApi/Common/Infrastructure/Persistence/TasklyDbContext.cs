using Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

namespace Taskly.WebApi.Common.Infrastructure.Persistence;

public sealed class TasklyDbContext(DbContextOptions<TasklyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TasklyDbContext).Assembly);
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.RegisterAllInEfcVogenIdConverter();
    }
}