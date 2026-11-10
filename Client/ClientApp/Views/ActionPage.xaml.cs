using ClientApp.ViewModels;

namespace ClientApp.Views;

public partial class ActionPage : ContentPage
{
    public ActionPage(ActionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}