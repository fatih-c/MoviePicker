using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string genreId)
        {
            var movies = await _movieService.GetMoviesAsync(genreId);
            return View("~/Views/Movie/Results.cshtml", movies);
        }
    }
}
