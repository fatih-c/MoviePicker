using Microsoft.AspNetCore.Mvc;
using MoviePicker.Models;
using MoviePicker.Services;

namespace MoviePicker.Controllers
{
    public class MovieController : Controller
    {
        private readonly MovieService _movieService;

        public MovieController(MovieService movieService)
        {
            _movieService = movieService;
        }
        public IActionResult Index()
        {
            return View(new MovieFilterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(MovieFilterViewModel filters)
        {
            filters.Movies = await _movieService.GetMoviesAsync(filters.SelectedGenres, 
                filters.MinRating, 
                filters.YearFrom, 
                filters.SortBy);
            return View(filters);
        }
    }
}
