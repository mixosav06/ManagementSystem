using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels;

public partial class ActionViewModel : ObservableObject
{
    [ObservableProperty]
    private int _spotID;

    [ObservableProperty]
    private int _userID;

    [RelayCommand]
    private async Task CreateActionAsync()
    {
        await Shell.Current.DisplayAlert("Успіх", $"Місце #{SpotID} заброньовано!", "OK");
        await Shell.Current.GoToAsync("..");
    }
}