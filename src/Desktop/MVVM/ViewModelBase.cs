using Desktop.MVVM.Settings.Models;

namespace Desktop.MVVM;

public abstract partial class ViewModelBase : ObservableObject
{
    public abstract string Title { get; }

    public static AppConfig AppConfig => AppConfig.Instance;
}
