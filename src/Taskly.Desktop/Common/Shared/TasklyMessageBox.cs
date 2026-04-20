using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace Taskly.Desktop.Common.Shared;

public static class TasklyMessageBox
{
    public static async Task<bool> ShowYesOrNoAsync(
        string caption,
        string message,
        CancellationToken cancellationToken = default)
    {
        var msg = new MessageBox
        {
            Title = caption,
            Content = message,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "Cancel",
            IsCloseButtonEnabled = false
        };

        return await msg.ShowDialogAsync(cancellationToken: cancellationToken) == MessageBoxResult.Primary;
    }
}