using Microsoft.AspNetCore.Http.Metadata;

namespace Taskly.Api.Attributes;


[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ControllerGroupAttribute(params string[] tags) : Attribute, ITagsMetadata
{
    public IReadOnlyList<string> Tags { get; } = tags;
}