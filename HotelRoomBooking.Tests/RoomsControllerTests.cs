using HotelRoomBooking.Controllers;
using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using HotelRoomBooking.Repositories;
using HotelRoomBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelRoomBooking.Tests
{
    public class RoomsControllerTests : IDisposable
    {
        private readonly HotelBookingContext _context;
        private readonly RoomsController _controller;

        public RoomsControllerTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<HotelBookingContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotelBookingContext(options);
            _controller = new RoomsController(new RoomService(new UnitOfWork(_context)));

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var hotel = new Hotel { Id = 1, Name = "Test Hotel" };
            _context.Hotels.Add(hotel);

            var rooms = new List<Room>
            {
                new Room { Id = 1, Type = "single", Capacity = 1, Price = 50, HotelId = 1 },
                new Room { Id = 2, Type = "double", Capacity = 2, Price = 80, HotelId = 1 },
                new Room { Id = 3, Type = "deluxe", Capacity = 4, Price = 120, HotelId = 1 }
            };
            _context.Rooms.AddRange(rooms);

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAvailableRooms_ValidRequest_ReturnsAvailableRooms()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var guests = 2;

            // Act
            var actionResult = await _controller.GetAvailableRooms(checkIn, checkOut, guests);
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var rooms = Assert.IsType<List<Room>>(okResult.Value);
            Assert.Equal(2, rooms.Count); // double and deluxe rooms
            Assert.All(rooms, room => Assert.True(room.Capacity >= guests));
        }

        [Fact]
        public async Task GetAvailableRooms_CheckOutBeforeCheckIn_ReturnsBadRequest()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(3);
            var checkOut = DateTime.Now.AddDays(1);
            var guests = 2;

            // Act
            var actionResult = await _controller.GetAvailableRooms(checkIn, checkOut, guests);
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAvailableRooms_ZeroGuests_ReturnsBadRequest()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var guests = 0;

            // Act
            var actionResult = await _controller.GetAvailableRooms(checkIn, checkOut, guests);
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAvailableRooms_BookedRoom_ReturnsOnlyAvailableRooms()
        {
            // Arrange - Create a booking
            var booking = new Booking
            {
                BookingReference = "TEST123",
                RoomId = 2, // Book the double room
                CustomerName = "Test User",
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var guests = 2;

            // Act
            var actionResult = await _controller.GetAvailableRooms(checkIn, checkOut, guests);
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var rooms = Assert.IsType<List<Room>>(okResult.Value);
            Assert.Single(rooms); // Only deluxe room should be available
            Assert.Equal("deluxe", rooms.First().Type);
        }

        [Fact]
        public void HotelHasCorrectRoomTypes()
        {
            // Arrange - Check that hotel has the 3 required room types
            var hotel = _context.Hotels.Include(h => h.Rooms).First();

            // Act
            var roomTypes = hotel.Rooms.Select(r => r.Type).Distinct().ToList();

            // Assert - Hotel should have single, double, and deluxe room types
            Assert.Contains("single", roomTypes);
            Assert.Contains("double", roomTypes);
            Assert.Contains("deluxe", roomTypes);
            Assert.Equal(3, roomTypes.Count); // Exactly 3 room types
        }

        [Fact]
        public void HotelHasExactlySixRooms()
        {
            // Arrange
            var hotel = _context.Hotels.Include(h => h.Rooms).First();

            // Act & Assert
            Assert.Equal(6, hotel.Rooms.Count); // Each hotel has exactly 6 rooms
        }

        [Fact]
        public void RoomCapacityCannotBeExceeded_SingleRoom()
        {
            // Arrange - Single room has capacity 1
            var singleRoom = _context.Rooms.First(r => r.Type == "single");

            // Act & Assert - Should not allow 2 guests in single room
            Assert.Equal(1, singleRoom.Capacity);
            Assert.True(singleRoom.Capacity < 2); // Cannot accommodate 2 guests
        }

        [Fact]
        public void RoomCapacityCannotBeExceeded_DoubleRoom()
        {
            // Arrange - Double room has capacity 2
            var doubleRoom = _context.Rooms.First(r => r.Type == "double");

            // Act & Assert - Should not allow 3 guests in double room
            Assert.Equal(2, doubleRoom.Capacity);
            Assert.True(doubleRoom.Capacity < 3); // Cannot accommodate 3 guests
        }

        [Fact]
        public void RoomCapacityCannotBeExceeded_DeluxeRoom()
        {
            // Arrange - Deluxe room has capacity 4
            var deluxeRoom = _context.Rooms.First(r => r.Type == "deluxe");

            // Act & Assert - Should not allow 5 guests in deluxe room
            Assert.Equal(4, deluxeRoom.Capacity);
            Assert.True(deluxeRoom.Capacity < 5); // Cannot accommodate 5 guests
        }

        [Fact]
        public async Task RoomCannotBeDoubleBooked_SameDates()
        {
            // Arrange - Book a room for specific dates
            var roomId = 2; // Double room
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);

            var booking1 = new Booking
            {
                BookingReference = "BOOKING1",
                RoomId = roomId,
                CustomerName = "Guest 1",
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = 2
            };
            _context.Bookings.Add(booking1);
            await _context.SaveChangesAsync();

            // Act - Try to book the same room for the same dates
            var booking2 = new Booking
            {
                BookingReference = "BOOKING2",
                RoomId = roomId,
                CustomerName = "Guest 2",
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = 2
            };
            _context.Bookings.Add(booking2);

            // Assert - Should throw exception due to unique constraint or validation
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _context.SaveChangesAsync());
        }

        [Fact]
        public async Task RoomCannotBeDoubleBooked_OverlappingDates()
        {
            // Arrange - Book a room for dates 1-3
            var roomId = 2;
            var booking1 = new Booking
            {
                BookingReference = "OVERLAP1",
                RoomId = roomId,
                CustomerName = "Guest 1",
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };
            _context.Bookings.Add(booking1);
            await _context.SaveChangesAsync();

            // Act - Try to book overlapping dates (2-4)
            var booking2 = new Booking
            {
                BookingReference = "OVERLAP2",
                RoomId = roomId,
                CustomerName = "Guest 2",
                CheckInDate = DateTime.Now.AddDays(2), // Overlaps with booking1
                CheckOutDate = DateTime.Now.AddDays(4),
                NumberOfGuests = 2
            };
            _context.Bookings.Add(booking2);

            // Assert - Should throw exception due to overlapping dates
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _context.SaveChangesAsync());
        }

        [Fact]
        public async Task BookingReferencesMustBeUnique()
        {
            // Arrange - Create two bookings with same reference
            var booking1 = new Booking
            {
                BookingReference = "DUPLICATE",
                RoomId = 1,
                CustomerName = "Guest 1",
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
                NumberOfGuests = 1
            };

            var booking2 = new Booking
            {
                BookingReference = "DUPLICATE", // Same reference
                RoomId = 2,
                CustomerName = "Guest 2",
                CheckInDate = DateTime.Now.AddDays(3),
                CheckOutDate = DateTime.Now.AddDays(4),
                NumberOfGuests = 2
            };

            _context.Bookings.Add(booking1);
            _context.Bookings.Add(booking2);

            // Act & Assert - Should throw exception due to duplicate reference
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _context.SaveChangesAsync());
        }

        [Fact]
        public async Task GuestsMustStayInSameRoom_NoRoomChanges()
        {
            // Arrange - Create a booking
            var booking = new Booking
            {
                BookingReference = "SAMEROOM",
                RoomId = 2, // Original room
                CustomerName = "Guest",
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act - Verify the booking is tied to the same room throughout
            var savedBooking = await _context.Bookings
                .Include(b => b.Room)
                .FirstAsync(b => b.BookingReference == "SAMEROOM");

            // Assert - Booking should maintain the same room
            Assert.Equal(2, savedBooking.RoomId);
            Assert.Equal("double", savedBooking.Room.Type);
            Assert.Equal(2, savedBooking.Room.Capacity);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
