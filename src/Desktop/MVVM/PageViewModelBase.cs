using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM;

public abstract partial class PageViewModelBase : ViewModelBase, INavigationAware
{
    private bool _isInitialized = false;

    public virtual Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
            _isInitialized = true;
        }

        return Task.CompletedTask;
    }

    protected virtual Task InitializeViewModel()
    {
        return Task.CompletedTask;
    }
}
