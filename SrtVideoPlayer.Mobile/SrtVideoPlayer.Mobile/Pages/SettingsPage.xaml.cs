using SrtVideoPlayer.Shared.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SrtVideoPlayer.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.Instance.ResolveViewModel<SettingsViewModel>();
        }

        private void SettingsPage_Appearing(object sender, EventArgs args) =>
            PreviewGrid.IsVisible = true;
    }
}
