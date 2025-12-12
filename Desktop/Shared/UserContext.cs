namespace Desktop.Shared;

public sealed class UserContext
{
    public string Id { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? PictureUrl { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}
