using Desktop.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Account;

/// <summary>
///     Interaktionslogik für AccountPage.xaml
/// </summary>
[RequiresAuthorizedUser]
[PageRegistration(typeof(AccountPageViewModel))]
public partial class AccountPage : INavigableView<AccountPageViewModel>
{
    public AccountPage(
        AccountPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public AccountPageViewModel ViewModel { get; }
}