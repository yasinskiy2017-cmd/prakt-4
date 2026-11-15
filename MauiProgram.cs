using MovieCatalogApp.Services;
using MovieCatalogApp.ViewModels;

namespace MovieCatalogApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Реєстрація HttpClient та API-сервісу в DI-контейнері
        builder.Services.AddSingleton(sp => new HttpClient
        {
            BaseAddress = new Uri("https://api.imdbapi.dev/")
        });
        builder.Services.AddSingleton<ImdbService>();

        // Реєстрація ViewModel та сторінки
        builder.Services.AddSingleton<MainPageModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
