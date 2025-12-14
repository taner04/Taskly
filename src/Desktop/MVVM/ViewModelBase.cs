namespace Desktop.MVVM;

public abstract partial class ViewModelBase : ObservableObject
{
    public abstract string Title { get; }
}
