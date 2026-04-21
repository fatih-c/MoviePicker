namespace MoviePicker.Models.Entities
{
    public class MovieSwipe
    {
        public int Id { get; set; }
        public int RoomMemberId { get; set; }
        public int MovieId { get; set; }
        public bool Liked { get; set; }
        public DateTime SwipedAt { get; set; }

        // Navigation property
        public RoomMember RoomMember { get; set; }
    }
}