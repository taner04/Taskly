using Desktop.Services.Auth0;
using Wpf.Ui;

namespace Desktop.MVVM.Account;

public sealed class AccountPageViewModel(
    ISnackbarService snackbarService,
    Auth0Service auth0Service) : PageViewModelBase(snackbarService, auth0Service)
{
    public override string Title => "Account";
}