namespace MoviePicker.Models.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string SelectedGenres { get; set; }
        public double MinRating { get; set; }
        public int YearFrom { get; set; }
        public string SortBy { get; set; }
        public List<RoomMember> Members { get; set; } = new();
    }
}