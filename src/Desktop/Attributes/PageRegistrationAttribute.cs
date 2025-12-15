using Desktop.MVVM;

namespace Desktop.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PageRegistrationAttribute : Attribute
{
    public PageRegistrationAttribute(
        Type viewModelType)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);

        if (!typeof(ViewModelBase).IsAssignableFrom(viewModelType))
        {
            throw new ArgumentException(
                $"The type '{viewModelType.FullName}' must inherit from '{nameof(ViewModelBase)}'",
                nameof(viewModelType));
        }

        ViewModelType = viewModelType;
    }

    public Type ViewModelType { get; }
}