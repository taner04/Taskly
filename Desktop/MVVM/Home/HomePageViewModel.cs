using Desktop.Services;
using Wpf.Ui;

namespace Desktop.MVVM.Home
{
    public partial class HomePageViewModel(
        INavigationService navigationService,
        Auth0Service auth0Service) : PageViewModelBase(navigationService, auth0Service)
    {
        public override string Title => "Home";
    }
}
