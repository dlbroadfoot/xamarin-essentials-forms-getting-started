using Xamarin.Essentials;

namespace RealEstateApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var windowsColor = Windows.UI.Color.FromArgb(255, 129, 66, 245);
            var systemColor = windowsColor.ToSystemColor();

            LoadApplication(new RealEstateApp.App(systemColor));
        }
    }
}
