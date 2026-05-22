using Temu_Catarig.Pages;

namespace Temu_Catarig
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for pages not in the TabBar
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
            Routing.RegisterRoute("ProductDetailPage", typeof(ProductDetailPage));
            Routing.RegisterRoute("MyOrdersPage", typeof(MyOrdersPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
            Routing.RegisterRoute("ContactUsPage", typeof(ContactUsPage));
            Routing.RegisterRoute("AddEditProductPage", typeof(AddEditProductPage));
            Routing.RegisterRoute("OrderFulfillmentPage", typeof(OrderFulfillmentPage));
            Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
            Routing.RegisterRoute("CartPage", typeof(CartPage));

            Routing.RegisterRoute("CheckoutPage", typeof(CheckoutPage));
            Routing.RegisterRoute("ManageProductsPage", typeof(ManageProductsPage));
            Routing.RegisterRoute("EditProfilePage", typeof(EditProfilePage));
        }
    }
}
