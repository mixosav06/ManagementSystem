using System.Collections.ObjectModel;
using ClientApp.Models;
using ClientApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels;

public partial class ItemsViewModel : ObservableObject
{
    private readonly ApiService _api;

    public ItemsViewModel(ApiService api) => _api = api;

    [ObservableProperty]
    public partial ObservableCollection<SpotItem> Items { get; set; } = new();

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    // Викликається зі сторінки в OnAppearing, тож список оновлюється після повернення з бронювання.
    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        var result = await _api.GetItemsAsync();
        IsBusy = false;

        if (result.Unauthorized)
        {
            await Shell.Current.DisplayAlertAsync("Сесія завершилась", "Увійдіть знову.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        if (!result.Success)
        {
            await Shell.Current.DisplayAlertAsync("Помилка", result.Error ?? "Не вдалося завантажити місця", "OK");
            return;
        }

        Items = new ObservableCollection<SpotItem>(result.Data!);
    }

    // Параметр необов'язковий: якщо передати вибране місце, воно підставиться на сторінку бронювання.
    [RelayCommand]
    private async Task GoToActionAsync(SpotItem? spot)
    {
        var route = spot is null ? "ActionPage" : $"ActionPage?SpotID={spot.ID}";
        await Shell.Current.GoToAsync(route);
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _api.Logout();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}