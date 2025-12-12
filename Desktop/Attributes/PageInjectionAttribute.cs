namespace Desktop.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PageInjectionAttribute(Type viewModel) : Attribute
{
    public Type ViewModel { get; } = viewModel;
}
