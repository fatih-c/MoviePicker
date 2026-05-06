using Microsoft.AspNetCore.Mvc;
using MoviePicker.Models;
using MoviePicker.Services;

namespace MoviePicker.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomService _roomService;
        private readonly MovieService _movieService;


        public RoomController(RoomService roomService, MovieService movieService)
        {
            _roomService = roomService;
            _movieService = movieService;
        }

        public IActionResult Create()
        {
            return View(new CreateRoomViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomViewModel model)
        {
            var room = await _roomService.CreateRoomAsync(model.CreatorName);

            var creator = room.Members.First(m => m.IsCreator);

            HttpContext.Session.SetInt32("MemberId", creator.Id);

            return RedirectToAction("Lobby", new { code = room.Code });
        }
        public IActionResult Join()
        {
            return View(new JoinRoomViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Join(JoinRoomViewModel model)
        {
            var (room, member) = await _roomService.JoinRoomAsync(model.Code, model.Name);

            if (room == null || member == null)
            {
                model.ErrorMessage = "Room not found or already started.";
                return View(model);
            }

            HttpContext.Session.SetInt32("MemberId", member.Id);

            return RedirectToAction("Lobby", new { code = room.Code });
        }
        [HttpPost]
        public async Task<IActionResult> SaveFilters(MovieFilterViewModel filters, string code)
        {
            var room = await _roomService.GetRoomWithMembersAsync(code);
            if (room == null) return NotFound();

            var memberId = HttpContext.Session.GetInt32("MemberId");
            var member = room.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null || !member.IsCreator) return RedirectToAction("Join");

            await _roomService.SaveFiltersAsync(code, filters);

            return RedirectToAction("Lobby", new { code });
        }
        [HttpPost]
        public async Task<IActionResult> Start(string code)
        {
            var memberId = HttpContext.Session.GetInt32("MemberId");
            if (memberId == null) return RedirectToAction("Join");

            await _roomService.StartRoomAsync(code, memberId.Value);

            return RedirectToAction("Swipe", new { code });
        }

        public async Task<IActionResult> Swipe(string code)
        {
            var room = await _roomService.GetRoomWithMembersAsync(code);
            if (room == null) return NotFound();

            var memberId = HttpContext.Session.GetInt32("MemberId");
            var member = room.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null) return RedirectToAction("Join");

            // Fetch movies based on room filters
            var genres = room.SelectedGenres?.Split(',').ToList() ?? new List<string>();
            var movies = await _movieService.GetMoviesAsync(
                genres,
                room.MinRating.ToString(),
                room.YearFrom.ToString(),
                room.SortBy ?? "popularity.desc",
                page: 1);

            ViewBag.Movies = movies;
            ViewBag.MemberId = memberId;
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> RecordSwipe([FromBody] RecordSwipeViewModel model)
        {
            await _roomService.RecordSwipeAsync(model.MemberId, model.MovieId, model.Liked);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> FinishSwiping([FromBody] FinishSwipingViewModel model)
        {
            await _roomService.FinishSwipingAsync(model.MemberId, model.RoomCode);
            return Ok();
        }
        [HttpGet]
        public IActionResult SetupParty()
        {
            // Ovo samo prikazuje View sa dvije opcije: Create ili Join
            return View();
        }
        public async Task<IActionResult> Lobby(string code)
        {
            var room = await _roomService.GetRoomWithMembersAsync(code);

            if (room == null) return NotFound();

            var memberId = HttpContext.Session.GetInt32("MemberId");
            var currentMember = room.Members.FirstOrDefault(m => m.Id == memberId);

            if (currentMember == null) return RedirectToAction("Join");

            ViewBag.IsCreator = currentMember.IsCreator;
            ViewBag.CurrentMember = currentMember;

            return View(room);
        }
    }
}
