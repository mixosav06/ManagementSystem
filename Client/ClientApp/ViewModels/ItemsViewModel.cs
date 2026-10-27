using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientApp.Models;

namespace ClientApp.ViewModels;

public partial class ItemsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<SpotItem> _items = new();

    public ItemsViewModel()
    {
        LoadMockItems();
    }

    [RelayCommand]
    private void LoadMockItems()
    {
        Items = new ObservableCollection<SpotItem>
        {
            new SpotItem { ID = 1, Number = 101, Status = "Free" },
            new SpotItem { ID = 2, Number = 102, Status = "Occupied" },
            new SpotItem { ID = 3, Number = 103, Status = "Free" }
        };
    }

    [RelayCommand]
    private async Task GoToActionAsync()
    {
        await Shell.Current.GoToAsync("ActionPage");
    }
}