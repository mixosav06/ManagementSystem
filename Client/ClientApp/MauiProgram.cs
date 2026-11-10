using ClientApp.Services;
using ClientApp.ViewModels;
using ClientApp.Views;
using Microsoft.Extensions.Logging;

namespace ClientApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Сервіси з підтримкою HTTPS для локального розробницького сервера
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var handler = new HttpClientHandler();
#if DEBUG
                // Дозволяє з'єднуватися з HTTPS локального сервера на етапі розробки
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
                return new HttpClient(handler);
            });

            builder.Services.AddSingleton<ApiService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ItemsViewModel>();
            builder.Services.AddTransient<ActionViewModel>();

            // Сторінки
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ItemsPage>();
            builder.Services.AddTransient<ActionPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}