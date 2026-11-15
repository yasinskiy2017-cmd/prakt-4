using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieCatalogApp.Models;
using MovieCatalogApp.Services;

namespace MovieCatalogApp.ViewModels;

public partial class MainPageModel : ObservableObject
{
    private readonly ImdbService _imdbService;

    public ObservableCollection<MovieModel> Movies { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isLoadingMore;

    [ObservableProperty]
    private string statusMessage = "Завантаження каталогу фільмів...";

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private int gridSpan = 1;

    public MainPageModel(ImdbService imdbService)
    {
        _imdbService = imdbService;
        _ = LoadInitialMoviesAsync();
    }

    public void UpdateOrientation(double width, double height)
    {
        if (width <= 0 || height <= 0)
            return;

        // У горизонтальній орієнтації (Landscape) відображаємо 2 колонки карток,
        // у вертикальній (Portrait) — 1 колонку.
        GridSpan = width > height ? 2 : 1;
    }

    [RelayCommand]
    public async Task LoadInitialMoviesAsync()
    {
        if (IsLoading)
            return;

        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = "Завантаження фільмів із IMDb API...";

            Movies.Clear();
            var items = await _imdbService.GetMoviesAsync(loadNextPage: false);

            foreach (var movie in items)
            {
                Movies.Add(movie);
            }

            StatusMessage = Movies.Count > 0
                ? $"Завантажено фільмів: {Movies.Count}"
                : "Список фільмів порожній.";
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Не вдалося завантажити дані: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadMoreMoviesAsync()
    {
        if (IsLoading || IsLoadingMore || Movies.Count == 0)
            return;

        try
        {
            IsLoadingMore = true;
            var moreItems = await _imdbService.GetMoviesAsync(loadNextPage: true);

            foreach (var movie in moreItems)
            {
                Movies.Add(movie);
            }

            StatusMessage = $"Завантажено фільмів: {Movies.Count}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка довантаження: {ex.Message}";
        }
        finally
        {
            IsLoadingMore = false;
        }
    }
}
