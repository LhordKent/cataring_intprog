using Microsoft.Extensions.Logging;
using Temu_Catarig.Services;
using Temu_Catarig.Pages;

namespace Temu_Catarig
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<FirebaseService>();
            builder.Services.AddSingleton<ImageService>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<LandingPage>();
            builder.Services.AddTransient<ProductDetailPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<AddEditProductPage>();
            builder.Services.AddTransient<AdminDashboardPage>();
            builder.Services.AddTransient<CheckoutPage>();
            builder.Services.AddTransient<MyOrdersPage>();
            builder.Services.AddTransient<ManageProductsPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<EditProfilePage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<OrderFulfillmentPage>();

            return builder.Build();
        }
    }
}
