namespace Temu_Catarig
{
    public partial class App : Application
    {
        public App(Services.AuthService authService)
        {
            InitializeComponent();
            _ = authService.InitializeAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}