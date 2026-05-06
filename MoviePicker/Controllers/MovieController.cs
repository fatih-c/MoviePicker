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
        /*  public IActionResult Index()
          {
              return View(new MovieFilterViewModel());
          }

          [HttpPost]
          public async Task<IActionResult> Index(MovieFilterViewModel filters)
          {
              filters.Movies = await _movieService.GetMoviesAsync(filters.SelectedGenres, 
                  filters.MinRating, 
                  filters.YearFrom, 
                  filters.SortBy,
                  filters.Page);
              return View(filters);
          }*/
        [HttpGet("Movie/Solo")]
        public IActionResult Solo()
        {
            return View(new MovieFilterViewModel());
        }

        [HttpPost("Movie/Solo")]
        public async Task<IActionResult> Solo(MovieFilterViewModel filters)
        {
            // Ako korisnik klikne "Find Movies" (obična lista)
            filters.Movies = await _movieService.GetMoviesAsync(
                filters.SelectedGenres,
                filters.MinRating,
                filters.YearFrom,
                filters.SortBy,
                filters.Page);

            return View(filters);
        }

        [HttpPost]
        public async Task<IActionResult> Swipe(MovieFilterViewModel filters)
        {
            filters.Movies = await _movieService.GetMoviesAsync(filters.SelectedGenres,
               filters.MinRating,
               filters.YearFrom,
               filters.SortBy,
               page: 1);
            return View(filters);
        }
    }
}
