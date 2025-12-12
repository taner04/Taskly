using Desktop.Attributes;
using Desktop.Services;
using Desktop.Shared;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM;

public abstract partial class PageViewModelBase(
    INavigationService navigationService,
    Auth0Service auth0Service) : ViewModelBase, INavigationAware
{
    protected bool _isInitialized;

    public UserContext? CurrentUser => auth0Service.CurrentUser;

    public virtual async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public virtual async Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
    }

    protected async Task<bool> NavigateAsync<TPage>()
        where TPage : INavigationAware
    {
        if (Attribute.IsDefined(
            typeof(TPage), 
            typeof(RequireAuthenticationAttribute)) && !auth0Service.IsAuthenticated)
        {
            return await auth0Service.LoginAsync();
        }

        navigationService.Navigate(typeof(TPage));
        return true;
    }

    protected virtual async Task InitializeAsync()
    {
        _isInitialized = true;
        await Task.CompletedTask;
    }


    protected void NavigateBack() 
        => navigationService.GoBack();
}
