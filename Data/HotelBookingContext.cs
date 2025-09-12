using HotelRoomBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Data
{
    public class HotelBookingContext : DbContext
    {
        public HotelBookingContext(DbContextOptions<HotelBookingContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany()
                .HasForeignKey(b => b.RoomId);

            // Seed data
            var hotel = new Hotel
            {
                Id = 1,
                Name = "Sample Hotel"
            };

            modelBuilder.Entity<Hotel>().HasData(hotel);

            var rooms = new List<Room>
            {
                new Room { Id = 1, Type = "single", Capacity = 1, Price = 50, HotelId = 1 },
                new Room { Id = 2, Type = "single", Capacity = 1, Price = 50, HotelId = 1 },
                new Room { Id = 3, Type = "double", Capacity = 2, Price = 80, HotelId = 1 },
                new Room { Id = 4, Type = "double", Capacity = 2, Price = 80, HotelId = 1 },
                new Room { Id = 5, Type = "deluxe", Capacity = 4, Price = 120, HotelId = 1 },
                new Room { Id = 6, Type = "deluxe", Capacity = 4, Price = 120, HotelId = 1 }
            };

            modelBuilder.Entity<Room>().HasData(rooms);
        }
    }
}
