namespace Desktop.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RequiresAuthorizedUserAttribute : Attribute;
