namespace IntegrationTests.Infrastructure;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase : IAsyncLifetime
{
    private readonly TestingFixture _fixture;
    private readonly IServiceScope _scope;

    protected TestingBase(TestingFixture fixture)
    {
        _fixture = fixture;
        _scope = _fixture.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!DbContext.Database.CanConnect())
        {
            throw new Exception("Test database is not reachable. Ensure the test container is running.");
        }
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

    protected HttpClient CreateClient()
    {
        return _fixture.CreateClient();
    }
}