using SrtVideoPlayer.Shared.ViewModels;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PreviewGrid.IsVisible = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            PreviewGrid.IsVisible = false;
        }
    }
}
