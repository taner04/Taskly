namespace Taskly.WebApi.Common.Abstractions;

public interface IEndpoint
{
    WebApplication MapEndpoint(WebApplication app);
}