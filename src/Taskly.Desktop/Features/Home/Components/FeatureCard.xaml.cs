using System.Windows.Controls;
using System.Windows.Media;

namespace Taskly.Desktop.Features.Home.Components;

public partial class FeatureCard : UserControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(FeatureCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(FeatureCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        nameof(Symbol), typeof(SymbolRegular), typeof(FeatureCard), new PropertyMetadata(SymbolRegular.Empty));

    public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register(
        nameof(IconForeground), typeof(Brush), typeof(FeatureCard), new PropertyMetadata(Brushes.Black));

    public FeatureCard()
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

    public SymbolRegular Symbol
    {
        get => (SymbolRegular)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public Brush IconForeground
    {
        get => (Brush)GetValue(IconForegroundProperty);
        set => SetValue(IconForegroundProperty, value);
    }
}