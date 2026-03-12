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

        public async Task<List<Movie>> GetMoviesAsync(string genreId)
        {

            string endpoint = $"discover/movie?api_key={apiKey}&with_genres={genreId}";

            var response = await _httpClient.GetFromJsonAsync<TmdbResponse>(endpoint);
            return response?.Movie ?? new List<Movie>();
        }
    }
}
