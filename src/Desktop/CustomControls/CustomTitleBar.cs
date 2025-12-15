using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace Desktop.CustomControls;

public sealed class CustomTitleBar : TitleBar
{
    public static readonly DependencyProperty CustomButtonContentProperty =
        DependencyProperty.Register(
            nameof(CustomButtonContent),
            typeof(object),
            typeof(CustomTitleBar),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CustomButtonCommandProperty =
        DependencyProperty.Register(
            nameof(CustomButtonCommand),
            typeof(ICommand),
            typeof(CustomTitleBar),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CustomButtonVisibilityProperty =
        DependencyProperty.Register(
            nameof(CustomButtonVisibility),
            typeof(Visibility),
            typeof(CustomTitleBar),
            new PropertyMetadata(Visibility.Visible));

    public object CustomButtonContent
    {
        get => GetValue(CustomButtonContentProperty);
        set => SetValue(CustomButtonContentProperty, value);
    }

    public ICommand CustomButtonCommand
    {
        get => (ICommand)GetValue(CustomButtonCommandProperty);
        set => SetValue(CustomButtonCommandProperty, value);
    }

    public Visibility CustomButtonVisibility
    {
        get => (Visibility)GetValue(CustomButtonVisibilityProperty);
        set => SetValue(CustomButtonVisibilityProperty, value);
    }

    public event RoutedEventHandler? CustomButtonClick;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_ButtonsPanel") is not Panel buttonsPanel)
        {
            return;
        }

        var customButton = new Button
        {
            Content = CustomButtonContent,
            Command = CustomButtonCommand,
            Visibility = CustomButtonVisibility,
            Margin = new Thickness(4, 0, 0, 0)
        };

        customButton.Click += (
            s,
            e) => CustomButtonClick?.Invoke(this, e);

        buttonsPanel.Children.Insert(0, customButton);
    }
}