using ClientApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _api;

    public LoginViewModel(ApiService api) => _api = api;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlertAsync("Помилка", "Введіть Email та пароль", "OK");
            return;
        }

        IsBusy = true;
        var result = await _api.LoginAsync(Email.Trim(), Password);
        IsBusy = false;

        if (!result.Success)
        {
            await Shell.Current.DisplayAlertAsync("Помилка входу", result.Error ?? "Невідома помилка", "OK");
            return;
        }

        Password = string.Empty;
        await Shell.Current.GoToAsync("//ItemsPage");
    }

    private bool CanLogin() => !IsBusy;
}