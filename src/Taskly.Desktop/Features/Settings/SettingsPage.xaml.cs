namespace Taskly.Desktop.Features.Settings;

public partial class SettingsPage : INavigableView<SettingsPageViewModel>
{
    public SettingsPage(SettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public SettingsPageViewModel ViewModel { get; }
}