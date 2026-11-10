using ClientApp.ViewModels;

namespace ClientApp.Views;

public partial class ItemsPage : ContentPage
{
    private readonly ItemsViewModel _vm;

    public ItemsPage(ItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Автоматично завантажує/оновлює список місць з API під час відкриття сторінки
        await _vm.LoadItemsCommand.ExecuteAsync(null);
    }
}