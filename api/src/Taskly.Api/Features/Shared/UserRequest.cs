using System.Text.Json.Serialization;
using Taskly.Api.Abstractions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Taskly.Api.Features.Shared;

public record UserRequest : IUserRequestBase
{
    [JsonIgnore] public string UserId { get; set; }
}