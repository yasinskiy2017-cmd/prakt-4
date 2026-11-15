namespace MovieCatalogApp.Models;

public class MovieModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = "Без назви";
    public string Description { get; set; } = "Опис відсутній";
    public string PosterUrl { get; set; } = string.Empty;
    public string ReleaseYear { get; set; } = "Невідомо";
    public string Rating { get; set; } = "—";
    public string Genres { get; set; } = string.Empty;
}
