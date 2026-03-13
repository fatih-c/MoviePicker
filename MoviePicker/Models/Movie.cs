using System.Text.Json.Serialization;

namespace MoviePicker.Models
{
    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int> GenreIds { get; set; } = new();

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = "";


        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = "";

        public string FullPosterUrl => string.IsNullOrEmpty(PosterPath) ? "/images/no-poster.png" : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}
