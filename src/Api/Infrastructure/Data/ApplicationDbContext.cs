using Api.Features.Users.Model;
using Api.Infrastructure.Data.Configuration;

namespace Api.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.RegisterAllInEfcVogenIdConverter();
    }
}