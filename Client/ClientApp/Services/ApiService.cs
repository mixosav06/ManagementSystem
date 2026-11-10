using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClientApp.Models;

namespace ClientApp.Services;

/// <summary>Результат виклику API без винятків назовні.</summary>
public record ApiResult<T>(bool Success, T? Data, string? Error, bool Unauthorized = false);

public class ApiService
{
    private const string TokenKey = "jwt_token";
    private const string UserNameKey = "user_name";

    private readonly HttpClient _http;

    // Android-емулятор бачитиме localhost комп'ютера як 10.0.2.2.
    // Порт 7292 працює через HTTPS (https://)
    public static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7292"
            : "https://localhost:7292";

    public ApiService(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri(BaseUrl);
        _http.Timeout = TimeSpan.FromSeconds(15);
    }

    // ---------- Робота з токеном ----------
    public string? Token => Preferences.Get(TokenKey, null);
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public void Logout()
    {
        Preferences.Remove(TokenKey);
        Preferences.Remove(UserNameKey);
    }

    // ---------- POST /api/auth/login ----------
    public async Task<ApiResult<LoginResponse>> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login",
                new LoginModel { Email = email, Password = password });

            if (!response.IsSuccessStatusCode)
                return new(false, null, await ReadErrorAsync(response), response.StatusCode == HttpStatusCode.Unauthorized);

            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (data is null || string.IsNullOrEmpty(data.Token))
                return new(false, null, "Сервер повернув некоректну відповідь.");

            Preferences.Set(TokenKey, data.Token);
            Preferences.Set(UserNameKey, data.Name);
            return new(true, data, null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new(false, null, "Не вдалося підключитися до сервера. Перевірте, чи він запущений.");
        }
    }

    // ---------- GET /api/items ----------
    public async Task<ApiResult<List<SpotItem>>> GetItemsAsync()
    {
        try
        {
            using var request = CreateRequest(HttpMethod.Get, "api/items");
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return Fail<List<SpotItem>>(response, await ReadErrorAsync(response));

            var items = await response.Content.ReadFromJsonAsync<List<SpotItem>>() ?? new();
            return new(true, items, null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new(false, null, "Не вдалося підключитися до сервера.");
        }
    }

    // ---------- POST /api/actions ----------
    public async Task<ApiResult<bool>> CreateActionAsync(int spotId)
    {
        try
        {
            using var request = CreateRequest(HttpMethod.Post, "api/actions");
            request.Content = JsonContent.Create(new CreateActionModel { SpotID = spotId });
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return Fail<bool>(response, await ReadErrorAsync(response));

            return new(true, true, null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new(false, false, "Не вдалося підключитися до сервера.");
        }
    }

    // ---------- Допоміжні ----------
    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        if (IsAuthenticated)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        return request;
    }

    private ApiResult<T> Fail<T>(HttpResponseMessage response, string error)
    {
        var unauthorized = response.StatusCode == HttpStatusCode.Unauthorized;
        if (unauthorized) Logout(); // токен недійсний або прострочений
        return new(false, default, unauthorized ? "Сесія завершилась. Увійдіть знову." : error, unauthorized);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var err = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (!string.IsNullOrWhiteSpace(err?.Message)) return err.Message!;
        }
        catch { /* тіло не JSON — повернемо загальне повідомлення */ }

        return $"Помилка сервера ({(int)response.StatusCode}).";
    }
}