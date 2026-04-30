using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MoviePicker.Data;
using MoviePicker.Hubs;
using MoviePicker.Models;
using MoviePicker.Models.Entities;
using System.CodeDom.Compiler;

namespace MoviePicker.Services
{
    public class RoomService
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<RoomHub> _hubContext;

        public RoomService(AppDbContext db, IHubContext<RoomHub> hub)
        {
            _db = db;
            _hubContext = hub;
        }

        public async Task<Room> CreateRoomAsync(string creatorName)
        {
            var room = new Room()
            {
                Code = GenerateCode(),
                Status = "Waiting",
                CreatedAt = DateTime.UtcNow,
                SelectedGenres = "",
                MinRating = 0,
                YearFrom = 2000,
                SortBy = "popularity.desc"
            };

            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();

            var creator = new RoomMember()
            {
                RoomId = room.Id,
                Name = creatorName,
                IsCreator = true,
                JoinedAt = DateTime.UtcNow,
                HasFinished = false
            };

            _db.RoomMembers.Add(creator);
            await _db.SaveChangesAsync();
            return room;
        }

        public async Task<(Room? room, RoomMember? member)> JoinRoomAsync(string code, string name)
        {
            var room = await _db.Rooms
                .Include(r => r.Members)
                .FirstOrDefaultAsync(r => r.Code == code);

            if (room == null) return (null, null);
            if (room.Status != "Waiting") return (null, null);

            var member = new RoomMember
            {
                RoomId = room.Id,
                Name = name,
                IsCreator = false,
                JoinedAt = DateTime.UtcNow,
                HasFinished = false
            };

            _db.RoomMembers.Add(member);
            await _db.SaveChangesAsync();

            await _hubContext.Clients.Group(room.Code)
                .SendAsync("MemberJoined", member.Name);

            return (room, member);
        }

        public async Task<Room?> GetRoomWithMembersAsync(string code)
        {
            return await _db.Rooms
                .Include(r => r.Members)
                .FirstOrDefaultAsync(r => r.Code == code);
        }

        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
        public async Task SaveFiltersAsync(string code, MovieFilterViewModel filters)
        {
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Code == code);
            if (room == null) return;

            room.SelectedGenres = string.Join(",", filters.SelectedGenres ?? new List<string>());
            room.MinRating = double.TryParse(filters.MinRating, out var rating) ? rating : 0;
            room.YearFrom = int.TryParse(filters.YearFrom, out var year) ? year : 0;
            room.SortBy = filters.SortBy ?? "popularity.desc";

            await _db.SaveChangesAsync();
        }
        public async Task<bool> StartRoomAsync(string code, int memberId)
        {
            var room = await _db.Rooms
                .Include(r => r.Members)
                .FirstOrDefaultAsync(r => r.Code == code);

            if (room == null) return false;

            var member = room.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null || !member.IsCreator) return false;

            room.Status = "Swiping";
            await _db.SaveChangesAsync();

            await _hubContext.Clients.Group(code)
                .SendAsync("GameStarted", code);

            return true;
        }

        public async Task RecordSwipeAsync(int memberId, int movieId, bool liked)
        {
            var swipe = new MovieSwipe
            {
                RoomMemberId = memberId,
                MovieId = movieId,
                Liked = liked,
                SwipedAt = DateTime.UtcNow
            };
            _db.MovieSwipes.Add(swipe);
            await _db.SaveChangesAsync();
        }

        public async Task FinishSwipingAsync(int memberId, string roomCode)
        {
            await _db.RoomMembers
                .Where(m => m.Id == memberId)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.HasFinished, true));
            var unfinishedMembers = await _db.RoomMembers
    .Where(m => m.Room.Code == roomCode && !m.HasFinished)
    .Select(m => m.Id)
    .ToListAsync();

            if (unfinishedMembers.Any())
            {
                // OVO ĆE TI REĆI KO TE KOČI
                Console.WriteLine($"Soba {roomCode}: Čekam na članove sa ID-ovima: {string.Join(", ", unfinishedMembers)}");
                return;
            }

            var isAnyoneStillSwiping = await _db.RoomMembers
                .AnyAsync(m => m.Room.Code == roomCode && !m.HasFinished);

            if (isAnyoneStillSwiping) return;

            //await Task.Delay(500);
            var rowsAffected = await _db.Rooms
                .Where(r => r.Code == roomCode && r.Status != "Finished")
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, "Finished"));

            if (rowsAffected == 0) return;

            var room = await _db.Rooms
                .Include(r => r.Members)
                .ThenInclude(m => m.Swipes)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == roomCode);

            if (room == null) return;

            var allFinished = room.Members.All(m => m.HasFinished);
            if (!allFinished) return;

            var memberCount = room.Members.Count;
            var likedByAll = room.Members
                .SelectMany(m => m.Swipes)
                .Where(s => s.Liked)
                .GroupBy(s => s.MovieId)
                .Where(g => g.Count() == memberCount)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList();

            await _db.Rooms
                .Where(r => r.Code == roomCode)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, "Finished"));

            await _hubContext.Clients.Group(roomCode)
                .SendAsync("ShowResults", likedByAll);

            await Task.Delay(5000);

            var memberIds = room.Members.Select(m => m.Id).ToList();

            await _db.MovieSwipes
                .Where(s => memberIds.Contains(s.RoomMemberId))
                .ExecuteDeleteAsync();

            await _db.RoomMembers
                .Where(m => memberIds.Contains(m.Id))
                .ExecuteDeleteAsync();

            await _db.Rooms
                .Where(r => r.Code == roomCode)
                .ExecuteDeleteAsync();
        }

    }
}
