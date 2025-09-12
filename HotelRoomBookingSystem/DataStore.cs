using HotelRoomBooking.Models;

namespace HotelRoomBooking
{
    public static class DataStore
    {
        public static List<Hotel> Hotels { get; set; } = new();
        public static List<Room> Rooms { get; set; } = new();
        public static List<Booking> Bookings { get; set; } = new();

        public static void SeedData()
        {
            Hotels.Clear();
            Rooms.Clear();
            Bookings.Clear();

            // Create a sample hotel
            var hotel = new Hotel
            {
                Id = 1,
                Name = "Sample Hotel",
                Rooms = new List<Room>()
            };
            Hotels.Add(hotel);

            // Add 6 rooms: 2 single, 2 double, 2 deluxe
            var rooms = new List<Room>
            {
                new Room { Id = 1, Type = "single", Capacity = 1, Price = 50, HotelId = 1, Hotel = hotel },
                new Room { Id = 2, Type = "single", Capacity = 1, Price = 50, HotelId = 1, Hotel = hotel },
                new Room { Id = 3, Type = "double", Capacity = 2, Price = 80, HotelId = 1, Hotel = hotel },
                new Room { Id = 4, Type = "double", Capacity = 2, Price = 80, HotelId = 1, Hotel = hotel },
                new Room { Id = 5, Type = "deluxe", Capacity = 4, Price = 120, HotelId = 1, Hotel = hotel },
                new Room { Id = 6, Type = "deluxe", Capacity = 4, Price = 120, HotelId = 1, Hotel = hotel }
            };

            foreach (var room in rooms)
            {
                Rooms.Add(room);
                hotel.Rooms.Add(room);
            }
        }

        public static void ResetData()
        {
            Hotels.Clear();
            Rooms.Clear();
            Bookings.Clear();
        }
    }
}
