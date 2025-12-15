using Desktop.Services.Auth0;
using Wpf.Ui;

namespace Desktop.MVVM;

public abstract class ViewModelBase(
    ISnackbarService snackbarService,
    Auth0Service auth0Service) : ObservableObject
{
    public abstract string Title { get; }

    public ISnackbarService SnackbarService => snackbarService;

    public Auth0Service Auth0Service => auth0Service;
}