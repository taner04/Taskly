using Desktop.Attributes;
using Desktop.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui;

namespace Desktop.MVVM;

[RequireAuthentication]
public abstract partial class AuthorizedPageViewModelBase(
    INavigationService navigationService,
    Auth0Service auth0Service) : PageViewModelBase(navigationService, auth0Service);
