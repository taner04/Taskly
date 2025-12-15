using Desktop.Services.Auth0;
using Wpf.Ui;

namespace Desktop.MVVM.Todo;

public sealed class TodoPageViewModel(
    ISnackbarService snackbarService,
    Auth0Service auth0Service) : PageViewModelBase(snackbarService, auth0Service)
{
    public override string Title => "Todo";
}