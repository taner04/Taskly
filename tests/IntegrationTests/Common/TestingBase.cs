using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace IntegrationTests.Common;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase : IAsyncLifetime
{
    private readonly TestingFixture _fixture;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _dbContext;

    protected TestingBase(TestingFixture fixture)
    {
        _fixture = fixture;
        _scope = _fixture.CreateScope();

        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!_dbContext.Database.CanConnect())
        {
            throw new NpgsqlException("Cannot connect to the database");
        }
    }

    protected static CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;
    
    protected ApplicationDbContext DbContext => _dbContext;

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
    
    protected void SetAuthenticatedUser(string userId)
    {
        var accessor = _scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

        accessor.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim("sub", userId)], "Test"))
        };
    }
}