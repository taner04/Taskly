using Taskly.Desktop.Common.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Taskly.Desktop.MVVM.Settings;

[PageInjection(typeof(SettingsPageViewModel))]
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
