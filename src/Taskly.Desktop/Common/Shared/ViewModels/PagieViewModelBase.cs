using Taskly.Desktop.Features.Main;

namespace Taskly.Desktop.Common.Shared.ViewModels;

public abstract class PagieViewModelBase(MainWindowViewModel mainWindowViewModel) : ObservableObject
{
    protected ISnackbarService SnackbarService
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            var snackbarService = App.Services.GetRequiredService<ISnackbarService>();

            if (App.Services.GetRequiredService<INavigationWindow>() is not MainWindow navigationWindow)
            {
                throw new InvalidOperationException("INavigationWindow is not a MainWindow.");
            }

            snackbarService.SetSnackbarPresenter(navigationWindow.SnackbarPresenter);
            field = snackbarService;

            return field;
        }
    }

    protected void SetProgressRingActive() => mainWindowViewModel.SetProgressRingActive();
    protected void SetProgressRingInactive() => mainWindowViewModel.SetProgressRingInactive();
}