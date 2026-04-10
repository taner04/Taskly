using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace Taskly.Desktop.Common;

public static class MessageBox
{
    public static async Task<bool> ShowYesOrNoAsync(
        string caption,
        string message,
        CancellationToken cancellationToken = default)
    {
        var msg = new Wpf.Ui.Controls.MessageBox
        {
            Title = caption,
            Content = message,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "Cancel",
            IsCloseButtonEnabled = false
        };

        return await msg.ShowDialogAsync(cancellationToken: cancellationToken) == MessageBoxResult.Primary;
    }

    public static async Task ShowErrorAsync(
        string caption,
        string message,
        CancellationToken cancellationToken = default)
    {
        var msg = new Wpf.Ui.Controls.MessageBox
        {
            Title = caption,
            Content = message,
            IsCloseButtonEnabled = true
        };

        await msg.ShowDialogAsync(cancellationToken: cancellationToken);
    }
}