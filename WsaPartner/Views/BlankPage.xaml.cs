using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

using WsaPartner.ViewModels;

namespace WsaPartner.Views
{
    public sealed partial class BlankPage : Page
    {
        public BlankViewModel ViewModel { get; }

        public BlankPage()
        {
            ViewModel = Ioc.Default.GetService<BlankViewModel>();
            InitializeComponent();
        }
    }
}
