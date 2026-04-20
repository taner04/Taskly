using System.Windows.Input;
using System.Windows.Media;

namespace Taskly.Desktop.Common.Shared.Behaviours;

public static class ListViewDoubleClickBehavior
{
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
            "Command",
            typeof(IRelayCommand),
            typeof(ListViewDoubleClickBehavior),
            new PropertyMetadata(null, OnCommandChanged));

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(ListViewDoubleClickBehavior),
            new PropertyMetadata(null, OnCommandChanged));

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(CommandProperty, value);
    }

    public static IRelayCommand GetCommand(DependencyObject obj) => (IRelayCommand)obj.GetValue(CommandProperty);

    public static void SetCommandParameter(DependencyObject obj, object value)
    {
        obj.SetValue(CommandParameterProperty, value);
    }

    public static object GetCommandParameter(DependencyObject obj) => obj.GetValue(CommandParameterProperty);

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListView listView)
        {
            if (e.OldValue != null)
            {
                listView.MouseDoubleClick -= ListView_MouseDoubleClick;
            }

            if (e.NewValue != null)
            {
                listView.MouseDoubleClick += ListView_MouseDoubleClick;
            }
        }
    }

    private static void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView listView)
        {
            var item = GetItemUnderMouse(listView, e);
            var command = GetCommand(listView);
            if (item != null && command != null && command.CanExecute(item))
            {
                command.Execute(item);
            }
        }
    }

    private static object GetItemUnderMouse(ListView listView, MouseButtonEventArgs e)
    {
        var depObj = e.OriginalSource as DependencyObject;
        while (depObj != null && depObj is not ListViewItem)
        {
            depObj = VisualTreeHelper.GetParent(depObj);
        }

        return (depObj as ListViewItem)?.DataContext!;
    }
}