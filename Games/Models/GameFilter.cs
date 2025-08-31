namespace Games.Models
{
    public class GameFilterViewModel
    {
        public string? Title { get; set; }
        public int? Year { get; set; }
        public bool UpcomingOnly { get; set; } = false;
        public List<int> SelectedGenreIds { get; set; } = new();

        public List<Genre> AllGenres { get; set; } = new();
        public List<Game> Games { get; set; } = new();
    }
}