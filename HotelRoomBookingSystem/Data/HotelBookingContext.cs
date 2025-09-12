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

            // Seed data - Hotels with exactly 6 rooms each, only single/double/deluxe types
            var hotels = new List<Hotel>
            {
                new Hotel { Id = 1, Name = "Grand Plaza Hotel" },
                new Hotel { Id = 2, Name = "Ocean View Resort" },
                new Hotel { Id = 3, Name = "City Center Inn" }
            };

            modelBuilder.Entity<Hotel>().HasData(hotels);

            var rooms = new List<Room>
            {
                // Grand Plaza Hotel (6 rooms: 2 single, 2 double, 2 deluxe)
                new Room { Id = 1, Type = "single", Capacity = 1, Price = 75, HotelId = 1 },
                new Room { Id = 2, Type = "single", Capacity = 1, Price = 75, HotelId = 1 },
                new Room { Id = 3, Type = "double", Capacity = 2, Price = 120, HotelId = 1 },
                new Room { Id = 4, Type = "double", Capacity = 2, Price = 120, HotelId = 1 },
                new Room { Id = 5, Type = "deluxe", Capacity = 4, Price = 200, HotelId = 1 },
                new Room { Id = 6, Type = "deluxe", Capacity = 4, Price = 200, HotelId = 1 },

                // Ocean View Resort (6 rooms: 2 single, 2 double, 2 deluxe)
                new Room { Id = 7, Type = "single", Capacity = 1, Price = 90, HotelId = 2 },
                new Room { Id = 8, Type = "single", Capacity = 1, Price = 90, HotelId = 2 },
                new Room { Id = 9, Type = "double", Capacity = 2, Price = 150, HotelId = 2 },
                new Room { Id = 10, Type = "double", Capacity = 2, Price = 150, HotelId = 2 },
                new Room { Id = 11, Type = "deluxe", Capacity = 4, Price = 250, HotelId = 2 },
                new Room { Id = 12, Type = "deluxe", Capacity = 4, Price = 250, HotelId = 2 },

                // City Center Inn (6 rooms: 2 single, 2 double, 2 deluxe)
                new Room { Id = 13, Type = "single", Capacity = 1, Price = 60, HotelId = 3 },
                new Room { Id = 14, Type = "single", Capacity = 1, Price = 60, HotelId = 3 },
                new Room { Id = 15, Type = "double", Capacity = 2, Price = 100, HotelId = 3 },
                new Room { Id = 16, Type = "double", Capacity = 2, Price = 100, HotelId = 3 },
                new Room { Id = 17, Type = "deluxe", Capacity = 4, Price = 160, HotelId = 3 },
                new Room { Id = 18, Type = "deluxe", Capacity = 4, Price = 160, HotelId = 3 }
            };

            modelBuilder.Entity<Room>().HasData(rooms);
        }
    }
}
