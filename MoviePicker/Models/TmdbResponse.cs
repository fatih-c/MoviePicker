using System.Text.Json.Serialization;

namespace MoviePicker.Models
{
    public class TmdbResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<Movie> Movie { get; set; } = new();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; } 
    }
}
