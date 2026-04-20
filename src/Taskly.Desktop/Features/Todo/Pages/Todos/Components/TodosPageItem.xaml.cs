using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Taskly.Desktop.Features.Tag.Models;
using Taskly.Desktop.Features.Todo.Models;
using Button = System.Windows.Controls.Button;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace Taskly.Desktop.Features.Todo.Pages.Todos.Components;

public partial class TodosPageItem : Card
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(TodosPageItem), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(TodosPageItem), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty TagsProperty = DependencyProperty.Register(
        nameof(Tags), typeof(ObservableCollection<TagModel>), typeof(TodosPageItem),
        new PropertyMetadata(default(ObservableCollection<TagModel>)));

    public static readonly DependencyProperty AttachmentsProperty = DependencyProperty.Register(
        nameof(Attachments), typeof(ObservableCollection<AttachmentModel>), typeof(TodosPageItem),
        new PropertyMetadata(default(ObservableCollection<AttachmentModel>)));

    public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(
        nameof(EditCommand), typeof(ICommand), typeof(TodosPageItem), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
        nameof(DeleteCommand), typeof(ICommand), typeof(TodosPageItem), new PropertyMetadata(default(ICommand)));

    public TodosPageItem()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public ObservableCollection<TagModel> Tags
    {
        get => (ObservableCollection<TagModel>)GetValue(TagsProperty);
        set => SetValue(TagsProperty, value);
    }

    public ObservableCollection<AttachmentModel> Attachments
    {
        get => (ObservableCollection<AttachmentModel>)GetValue(AttachmentsProperty);
        set => SetValue(AttachmentsProperty, value);
    }

    public ICommand EditCommand
    {
        get => (ICommand)GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    private void Expander_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // 1. Check if we clicked the toggle button (the arrow)
        var obj = (DependencyObject)e.OriginalSource;
        var isToggleButton = false;

        while (obj != null && obj != (DependencyObject)sender)
        {
            if (obj is ToggleButton)
            {
                isToggleButton = true;
                break;
            }

            obj = VisualTreeHelper.GetParent(obj);
        }

        // 2. If it's NOT the toggle button, we handle the event ourselves
        if (isToggleButton)
        {
            return;
        }

        // Mark as handled so the Expander ITSELF doesn't see it (won't toggle)
        e.Handled = true;

        // Find the ListViewItem to manage selection and double-click
        var dep = (DependencyObject)e.OriginalSource;

        // Skip logic if we clicked a button (Edit/Delete)
        if (dep is { } source &&
            (source is Button ||
             VisualTreeHelper.GetParent(source) is Button ||
             source is Wpf.Ui.Controls.Button ||
             VisualTreeHelper.GetParent(source) is Wpf.Ui.Controls.Button))
        {
            e.Handled = false; // Let the button handle its own click
            return;
        }

        while (dep != null && dep is not ListViewItem)
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (dep is not ListViewItem listViewItem)
        {
            return;
        }

        var listView = VisualTreeHelper.GetParent(listViewItem);
        while (listView != null && listView is not ListView)
        {
            listView = VisualTreeHelper.GetParent(listView);
        }

        if (listView is not ListView lv)
        {
            return;
        }

        // Update selection manually
        lv.SelectedItem = listViewItem.DataContext;

        if (e.ClickCount != 2)
        {
            return;
        }

        // Raise DoubleClick for the Edit Behavior
        var eventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = MouseDoubleClickEvent,
            Source = listViewItem
        };
        listViewItem.RaiseEvent(eventArgs);
    }
}