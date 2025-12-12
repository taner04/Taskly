using Desktop.Attributes;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Tag
{
    /// <summary>
    /// Interaktionslogik für TagPage.xaml
    /// </summary>
    [RequireAuthentication]
    [PageInjection(typeof(TagPageViewModel))]
    public partial class TagPage : INavigableView<TagPageViewModel>
    {
        public TagPageViewModel ViewModel { get; }

        public TagPage(TagPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
