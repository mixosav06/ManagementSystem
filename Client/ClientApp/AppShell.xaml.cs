using ClientApp.Views;

namespace ClientApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ActionPage відкривається поверх ItemsPage (навігація "ActionPage?SpotID=3")
        Routing.RegisterRoute(nameof(ActionPage), typeof(ActionPage));
    }
}
