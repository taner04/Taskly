using Taskly.IntegrationTests.Infrastructure.Composition.Mocks.Jwt;
using Taskly.IntegrationTests.Infrastructure.Fixtures;
using Taskly.WebApi.Client.Abstractions;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.IntegrationTests.Infrastructure;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase(TestingFixture fixture) : IAsyncLifetime
{
    private IServiceScope _scope = null!;

    protected UserId CurrentUserId { get; private set; }

    protected static CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await fixture.SetUpAsync();

        _scope = fixture.CreateScope();

        CurrentUserId = await GetDbContext().Users
            .Select(u => u.Id)
            .FirstAsync();
    }

    public ValueTask DisposeAsync()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    protected TasklyDbContext GetDbContext() => _scope.ServiceProvider.GetRequiredService<TasklyDbContext>();

    protected IApiClient CreateAuthenticatedUserClient() => fixture.CreateAuthenticatedClient(UserRole.User);

    protected IApiClient CreateAuthenticatedAdminClient() => fixture.CreateAuthenticatedClient(UserRole.Admin);

    protected IApiClient CreateUnauthenticatedClient() => fixture.CreateUnauthenticatedClient();

    protected async Task<UserId> CreateForeignUserAsync() => await fixture.CreateForeignUserAsync();

    protected T GetService<T>() where T : class => _scope.ServiceProvider.GetRequiredService<T>();
}