namespace Desktop.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class RequireAuthenticationAttribute : Attribute;
