namespace MoviePicker.Models
{
    public class MovieFilterViewModel
    {
        public List<string> SelectedGenres { get; set; } = new();
        public string MinRating { get; set; } = "5";
        public string YearFrom { get; set; } = "";
        public string SortBy { get; set; } = "popularity.desc";
        public int Page{ get; set; } = 1;
        public List<Movie> Movies { get; set; } = new();
    }
}
