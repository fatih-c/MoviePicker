namespace MoviePicker.Models.Entities
{
    public class RoomMember
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool HasFinished { get; set; }

        // Navigation properties
        public Room Room { get; set; }
        public List<MovieSwipe> Swipes { get; set; } = new();
    }
}