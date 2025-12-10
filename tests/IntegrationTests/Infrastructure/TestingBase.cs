using Api;
using Api.Features.Users.Model;

namespace IntegrationTests.Infrastructure;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase : IAsyncLifetime
{
    private readonly TestingFixture _fixture;
    private readonly IServiceScope _scope;

    protected TestingBase(
        TestingFixture fixture)
    {
        _fixture = fixture;
        _scope = _fixture.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected static CancellationToken CurrentCancellationToken => TestsContext.CurrentCancellationToken;

    protected ApplicationDbContext DbContext { get; }

    public async ValueTask InitializeAsync()
    {
        await _fixture.SetUpAsync();
    }

    public ValueTask DisposeAsync()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    protected IApiClient CreateAuthenticatedClient()
    {
        return _fixture.CreateAuthenticatedClient();
    }

    protected IApiClient CreateUnauthenticatedClient()
    {
        return _fixture.CreateUnauthenticatedClient();
    }

    protected UserId GetCurrentUserId()
    {
        return _fixture.GetCurrentUserId();
    }

    protected T GetService<T>()
        where T : notnull
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }
}