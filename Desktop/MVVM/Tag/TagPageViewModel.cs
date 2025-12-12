using Desktop.Services;
using Wpf.Ui;

namespace Desktop.MVVM.Tag;

public sealed class TagPageViewModel(
    INavigationService navigationService, 
    Auth0Service auth0Service) : AuthorizedPageViewModelBase(navigationService, auth0Service)
{
    public override string Title => "Tags";
}
