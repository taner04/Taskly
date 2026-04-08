using Microsoft.EntityFrameworkCore.Design;
using Taskly.ServiceDefaults;

namespace Taskly.WebApi.Common.Infrastructure.Persistence;

public sealed class TasklyDbContextFactory : IDesignTimeDbContextFactory<TasklyDbContext>
{
    public TasklyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TasklyDbContext>();

        optionsBuilder.UseNpgsql(AppHostConstants.Database);

        return new TasklyDbContext(optionsBuilder.Options);
    }
}
