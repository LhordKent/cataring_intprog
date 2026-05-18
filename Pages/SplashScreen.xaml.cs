using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Temu_Catarig.Pages
{
    public partial class SplashScreen : ContentPage
    {
        public SplashScreen()
        {
            InitializeComponent();
            _ = NavigateToLogin();
        }

        private async Task NavigateToLogin()
        {
            await Task.Delay(3000); // 3 seconds delay
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
