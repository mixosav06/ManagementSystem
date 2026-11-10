using ClientApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels;

[QueryProperty(nameof(SpotID), "SpotID")]
public partial class ActionViewModel : ObservableObject
{
    private readonly ApiService _api;

    public ActionViewModel(ApiService api) => _api = api;

    [ObservableProperty]
    public partial int SpotID { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateActionCommand))]
    public partial bool IsBusy { get; set; }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private async Task CreateActionAsync()
    {
        if (SpotID <= 0)
        {
            await Shell.Current.DisplayAlertAsync("Помилка", "Вкажіть ID паркувального місця", "OK");
            return;
        }

        IsBusy = true;
        var result = await _api.CreateActionAsync(SpotID);
        IsBusy = false;

        if (result.Unauthorized)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        if (!result.Success)
        {
            await Shell.Current.DisplayAlertAsync("Помилка", result.Error ?? "Не вдалося забронювати", "OK");
            return;
        }

        await Shell.Current.DisplayAlertAsync("Успіх", $"Місце #{SpotID} заброньовано!", "OK");
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool CanCreate() => !IsBusy;
}