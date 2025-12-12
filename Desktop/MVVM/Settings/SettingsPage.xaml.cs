using Desktop.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Settings;


[PageInjection(typeof(SettingsPageViewModel))]
public partial class SettingsPage : INavigableView<SettingsPageViewModel>
{
    public SettingsPageViewModel ViewModel { get; }

    public SettingsPage(SettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
