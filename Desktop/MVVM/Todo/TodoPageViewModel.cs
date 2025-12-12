using Desktop.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui;

namespace Desktop.MVVM.Todo;

public sealed class TodoPageViewModel(
    INavigationService navigationService,
    Auth0Service auth0Service) : AuthorizedPageViewModelBase(navigationService, auth0Service)
{
    public override string Title => "Todos";
}
