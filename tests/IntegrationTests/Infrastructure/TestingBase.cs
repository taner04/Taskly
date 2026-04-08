using Api.Client;
using Api.Common.Infrastructure.Persistence;
using IntegrationTests.Infrastructure.Mocks.Jwt;

namespace IntegrationTests.Infrastructure;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase(TestingFixture fixture) : IAsyncLifetime
{
    private IServiceScope _scope = null!;

    protected UserId CurrentUserId { get; private set; }

    protected static CancellationToken CurrentCancellationToken => TestsContext.CurrentCancellationToken;

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

    protected ApplicationDbContext GetDbContext() => _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    protected IApiClient CreateAuthenticatedUserClient() => fixture.CreateAuthenticatedClient(UserRole.User);

    protected IApiClient CreateAuthenticatedAdminClient() => fixture.CreateAuthenticatedClient(UserRole.Admin);

    protected IApiClient CreateUnauthenticatedClient() => fixture.CreateUnauthenticatedClient();

    protected async Task<UserId> CreateForeignUserAsync() => await fixture.CreateForeignUserAsync();

    protected T GetService<T>() where T : class => _scope.ServiceProvider.GetRequiredService<T>();
}