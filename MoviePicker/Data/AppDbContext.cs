using Microsoft.EntityFrameworkCore;
using MoviePicker.Models.Entities;
using System.Collections.Generic;

namespace MoviePicker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomMember> RoomMembers { get; set; }
        public DbSet<MovieSwipe> MovieSwipes { get; set; }
    }
}
