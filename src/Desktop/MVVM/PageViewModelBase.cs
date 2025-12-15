using Desktop.Services.Auth0;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM;

public abstract class PageViewModelBase(
    ISnackbarService snackbarService,
    Auth0Service auth0Service) : ViewModelBase(
    snackbarService,
    auth0Service), INavigationAware, IDisposable
{
    private bool _isInitialized;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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

    protected virtual void Dispose(
        bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources here if any
        }
    }
}