namespace Taskly.WebApi.Client.Features.Tags.Dtos;

public sealed record CreateTagRequest
{
    public required string TagName { get; init; }
}