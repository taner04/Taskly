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

    protected static CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;

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

    protected HttpClient CreateAuthenticatedClient()
    {
        return _fixture.CreateAuthenticatedClient();
    }

    protected HttpClient CreateUnauthenticatedClient()
    {
        return _fixture.CreateUnauthenticatedClient();
    }
}