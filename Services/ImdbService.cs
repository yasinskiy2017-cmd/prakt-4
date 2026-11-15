using System.Text.Json;
using System.Text.Json.Nodes;
using MovieCatalogApp.Models;

namespace MovieCatalogApp.Services;

public class ImdbService
{
    private readonly HttpClient _httpClient;
    private string? _nextPageToken;
    private bool _hasMore = true;

    public ImdbService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri("https://api.imdbapi.dev/");
        }
        _httpClient.Timeout = TimeSpan.FromSeconds(20);
    }

    public void ResetPagination()
    {
        _nextPageToken = null;
        _hasMore = true;
    }

    public async Task<List<MovieModel>> GetMoviesAsync(bool loadNextPage = false)
    {
        var movies = new List<MovieModel>();

        if (!loadNextPage)
        {
            ResetPagination();
        }
        else if (!_hasMore)
        {
            return movies;
        }

        try
        {
            var endpoint = string.IsNullOrWhiteSpace(_nextPageToken)
                ? "titles"
                : $"titles?pageToken={Uri.EscapeDataString(_nextPageToken)}";

            using var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonText = await response.Content.ReadAsStringAsync();
            var root = JsonNode.Parse(jsonText)?.AsObject();

            if (root == null)
                return movies;

            // Оновлюємо токен пагінації (imdbapi.dev використовує nextPageToken)
            var nextTokenNode = root["nextPageToken"] ?? root["next_page_token"];
            _nextPageToken = nextTokenNode?.ToString();
            _hasMore = !string.IsNullOrWhiteSpace(_nextPageToken);

            // Шукаємо масив елементів у "titles" або "results"
            JsonArray? items = null;
            if (root["titles"] is JsonArray titlesArray)
                items = titlesArray;
            else if (root["results"] is JsonArray resultsArray)
                items = resultsArray;

            if (items == null)
                return movies;

            foreach (var item in items)
            {
                if (item is not JsonObject obj)
                    continue;

                var id = obj["id"]?.ToString() ?? Guid.NewGuid().ToString();
                var title = obj["primaryTitle"]?.ToString()
                            ?? obj["originalTitle"]?.ToString()
                            ?? obj["title"]?.ToString()
                            ?? "Без назви";

                var description = obj["plot"]?.ToString()
                                  ?? obj["description"]?.ToString()
                                  ?? "Опис для цього фільму відсутній.";

                var posterUrl = obj["primaryImage"]?["url"]?.ToString()
                                ?? obj["image"]?.ToString()
                                ?? string.Empty;

                var startYear = obj["startYear"]?.ToString()
                                ?? obj["year"]?.ToString()
                                ?? "—";

                string ratingText = "—";
                if (obj["rating"] is JsonObject ratingObj && ratingObj["aggregateRating"] != null)
                {
                    ratingText = $"★ {ratingObj["aggregateRating"]}";
                }

                string genresText = string.Empty;
                if (obj["genres"] is JsonArray genresArr && genresArr.Count > 0)
                {
                    var genreList = genresArr
                        .Select(g => g?.ToString())
                        .Where(g => !string.IsNullOrWhiteSpace(g))
                        .Take(3);
                    genresText = string.Join(", ", genreList!);
                }

                movies.Add(new MovieModel
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    PosterUrl = posterUrl,
                    ReleaseYear = startYear,
                    Rating = ratingText,
                    Genres = genresText
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImdbService] Помилка запиту до API: {ex.Message}");
            throw;
        }

        return movies;
    }
}
