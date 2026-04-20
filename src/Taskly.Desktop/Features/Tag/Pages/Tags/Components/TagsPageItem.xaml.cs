using System.Windows.Input;

namespace Taskly.Desktop.Features.Tag.Pages.Tags.Components;

public partial class TagsPageItem : Card
{
    public static readonly DependencyProperty TagNameProperty = DependencyProperty.Register(
        nameof(TagName),
        typeof(string),
        typeof(TagsPageItem),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(
        nameof(EditCommand), typeof(ICommand), typeof(TagsPageItem), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
        nameof(DeleteCommand), typeof(ICommand), typeof(TagsPageItem), new PropertyMetadata(default(ICommand)));

    public TagsPageItem()
    {
        InitializeComponent();
    }

    public string TagName
    {
        get => (string)GetValue(TagNameProperty);
        set => SetValue(TagNameProperty, value);
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
}