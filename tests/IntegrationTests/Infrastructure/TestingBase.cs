using Api;
using Api.Features.Shared;

namespace IntegrationTests.Infrastructure;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase(TestingFixture fixture) : IAsyncLifetime
{
    private IServiceScope _scope = null!;

    private ApplicationDbContext DbContext { get; set; } = null!;
    protected UserId CurrentUserId { get; private set; }

    protected static CancellationToken CurrentCancellationToken => TestsContext.CurrentCancellationToken;

    public async ValueTask InitializeAsync()
    {
        await fixture.SetUpAsync();

        _scope = fixture.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        CurrentUserId = await DbContext.Users
            .Select(u => u.Id)
            .FirstAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _scope.Dispose();
        await DbContext.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected ApplicationDbContext GetDbContext()
    {
        return DbContext;
    }

    protected IApiClient CreateAuthenticatedUserClient()
    {
        return fixture.CreateAuthenticatedClient(Policies.User);
    }

    protected IApiClient CreateAuthenticatedAdminClient()
    {
        return fixture.CreateAuthenticatedClient(Policies.Admin);
    }

    protected IApiClient CreateUnauthenticatedClient()
    {
        return fixture.CreateUnauthenticatedClient();
    }

    protected T GetService<T>() where T : class
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }
}