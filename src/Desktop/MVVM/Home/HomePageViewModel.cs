using Desktop.MVVM.Home.Models;
using Desktop.Services.Auth0;
using Wpf.Ui;

namespace Desktop.MVVM.Home;

public sealed partial class HomePageViewModel(
    ISnackbarService snackbarService,
    Auth0Service auth0Service)
    : PageViewModelBase(snackbarService, auth0Service)
{
    [ObservableProperty] private List<FeatureItem> _featureItems = [];
    [ObservableProperty] private string _welcomeMessage = "Welcome to Taskly Desktop!";

    public override string Title => "Home";

    public string ActionBtnTxt => Auth0Service.IsAuthenticated ? "Logout" : "Login";

    private void OnAuthStateChanged(
        UserContext? currentUser)
    {
        OnPropertyChanged(nameof(ActionBtnTxt));
    }

    protected override Task InitializeViewModel()
    {
        Auth0Service.AuthenticationStateChanged += OnAuthStateChanged;

        FeatureItems =
        [
            new FeatureItem
            {
                Icon = "⚡",
                Title = "Fast & Simple",
                Description = "Add, edit, and organize tasks instantly."
            },
            new FeatureItem
            {
                Icon = "🎯",
                Title = "Stay on Track",
                Description = "Set priorities, deadlines, and reminders that actually help."
            },
            new FeatureItem
            {
                Icon = "📊",
                Title = "See Your Progress",
                Description = "Visualize what's done and what's next."
            },
            new FeatureItem
            {
                Icon = "🌍",
                Title = "Works Everywhere",
                Description = "Access your tasks anytime, on any device."
            },
            new FeatureItem
            {
                Icon = "✅",
                Title = "Track Completion",
                Description = "Mark tasks as done and watch your productivity grow with each check."
            },
            new FeatureItem
            {
                Icon = "🔓",
                Title = "100% Open Source",
                Description = "Free, transparent, and community-driven."
            }
        ];
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (Auth0Service.IsAuthenticated)
        {
            await Auth0Service.LogoutAsync();
        }
        else
        {
            await Auth0Service.LoginAsync();
        }
    }

    protected override void Dispose(
        bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            Auth0Service.AuthenticationStateChanged -= OnAuthStateChanged;
        }
    }
}