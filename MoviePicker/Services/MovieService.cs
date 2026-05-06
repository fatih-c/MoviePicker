using MoviePicker.Models;
using Microsoft.Extensions.Configuration;   

namespace MoviePicker.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey;
        public MovieService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            apiKey = configuration["TMDB:ApiKey"];
        }

        public async Task<List<Movie>> GetMoviesAsync(List<string> genreIds, string minRating, string yearFrom, string sortBy, int page = 1)
        {
            string genres = string.Join(",", genreIds);
            string endpoint = $"discover/movie?api_key={apiKey}"+
                $"&with_genres={genres}"+
                $"&vote_average.gte={minRating}"+
                $"&sort_by={sortBy}"+
                $"&vote_count.gte=1000"+ 
                $"&page={page}";

            if (!string.IsNullOrEmpty(yearFrom))
                endpoint += $"&primary_release_date.gte={yearFrom}-01-01";

            Console.WriteLine($"Calling TMDB: {endpoint}");

            var response = await _httpClient.GetFromJsonAsync<TmdbResponse>(endpoint);

            Console.WriteLine($"Movies returned: {response?.Movie?.Count}");
            return response?.Movie ?? new List<Movie>();
        }
    }
}
