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
    public class BookingsControllerTests : IDisposable
    {
        private readonly HotelBookingContext _context;
        private readonly BookingsController _controller;

        public BookingsControllerTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<HotelBookingContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotelBookingContext(options);
            _controller = new BookingsController(new BookingService(new UnitOfWork(_context)));

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var hotel = new Hotel { Id = 1, Name = "Test Hotel" };
            _context.Hotels.Add(hotel);

            var rooms = new List<Room>
            {
                // 6 rooms total: 2 single, 2 double, 2 deluxe
                new Room { Id = 1, Type = "single", Capacity = 1, Price = 50, HotelId = 1 },
                new Room { Id = 2, Type = "single", Capacity = 1, Price = 50, HotelId = 1 },
                new Room { Id = 3, Type = "double", Capacity = 2, Price = 80, HotelId = 1 },
                new Room { Id = 4, Type = "double", Capacity = 2, Price = 80, HotelId = 1 },
                new Room { Id = 5, Type = "deluxe", Capacity = 4, Price = 120, HotelId = 1 },
                new Room { Id = 6, Type = "deluxe", Capacity = 4, Price = 120, HotelId = 1 }
            };
            _context.Rooms.AddRange(rooms);

            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateBooking_ValidRequest_ReturnsCreatedBooking()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomId = 3, // Double room, capacity 2
                CustomerName = "John Doe",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };

            // Act
            var actionResult = await _controller.CreateBooking(request);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var booking = Assert.IsType<Booking>(createdResult.Value);
            Assert.Equal("John Doe", booking.CustomerName);
            Assert.Equal(3, booking.RoomId);
            Assert.Equal(2, booking.NumberOfGuests);
            Assert.NotNull(booking.BookingReference);
            Assert.True(booking.BookingReference.Length == 8); // GUID substring
        }

        [Fact]
        public async Task CreateBooking_ExceedsRoomCapacity_ReturnsBadRequest()
        {
            // Arrange - Try to book 3 guests in a double room (capacity 2)
            var request = new BookingRequest
            {
                RoomId = 2, // Double room capacity = 2
                CustomerName = "John Doe",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 3 // Exceeds capacity
            };

            // Act
            var actionResult = await _controller.CreateBooking(request);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateBooking_CheckOutBeforeCheckIn_ReturnsBadRequest()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomId = 2,
                CustomerName = "John Doe",
                CheckIn = DateTime.Now.AddDays(3),
                CheckOut = DateTime.Now.AddDays(1), // Before check-in
                NumberOfGuests = 2
            };

            // Act
            var actionResult = await _controller.CreateBooking(request);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateBooking_ZeroGuests_ReturnsBadRequest()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomId = 2,
                CustomerName = "John Doe",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 0 // Invalid
            };

            // Act
            var actionResult = await _controller.CreateBooking(request);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateBooking_InvalidRoomId_ReturnsNotFound()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomId = 999, // Non-existent room
                CustomerName = "John Doe",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 1
            };

            // Act
            var actionResult = await _controller.CreateBooking(request);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateBooking_DoubleBookingSameDates_ReturnsConflict()
        {
            // Arrange - First booking
            var request1 = new BookingRequest
            {
                RoomId = 3, // Double room
                CustomerName = "Guest 1",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };
            await _controller.CreateBooking(request1);

            // Second booking for same dates
            var request2 = new BookingRequest
            {
                RoomId = 3, // Same room
                CustomerName = "Guest 2",
                CheckIn = DateTime.Now.AddDays(1), // Same dates
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };

            // Act
            var actionResult = await _controller.CreateBooking(request2);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task CreateBooking_OverlappingDates_ReturnsConflict()
        {
            // Arrange - First booking: days 1-3
            var request1 = new BookingRequest
            {
                RoomId = 3, // Double room
                CustomerName = "Guest 1",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };
            await _controller.CreateBooking(request1);

            // Second booking: days 2-4 (overlaps)
            var request2 = new BookingRequest
            {
                RoomId = 3, // Same room
                CustomerName = "Guest 2",
                CheckIn = DateTime.Now.AddDays(2), // Overlaps
                CheckOut = DateTime.Now.AddDays(4),
                NumberOfGuests = 2
            };

            // Act
            var actionResult = await _controller.CreateBooking(request2);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task GetBooking_ExistingReference_ReturnsBooking()
        {
            // Arrange - Create a booking first
            var createRequest = new BookingRequest
            {
                RoomId = 3, // Double room
                CustomerName = "John Doe",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(3),
                NumberOfGuests = 2
            };
            var createResult = await _controller.CreateBooking(createRequest);
            var createdResult = Assert.IsType<CreatedAtActionResult>(createResult.Result);
            var createdBooking = Assert.IsType<Booking>(createdResult.Value);
            var reference = createdBooking.BookingReference;

            // Act
            var actionResult = await _controller.GetBooking(reference);
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var booking = Assert.IsType<Booking>(okResult.Value);
            Assert.Equal(reference, booking.BookingReference);
            Assert.Equal("John Doe", booking.CustomerName);
            Assert.Equal(3, booking.RoomId);
        }

        [Fact]
        public async Task GetBooking_NonExistingReference_ReturnsNotFound()
        {
            // Act
            var actionResult = await _controller.GetBooking("NONEXISTENT");
            var result = actionResult.Result; // Unwrap ActionResult

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task BookingReferenceIsUnique()
        {
            // Arrange - Create first booking
            var request1 = new BookingRequest
            {
                RoomId = 1,
                CustomerName = "Guest 1",
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(2),
                NumberOfGuests = 1
            };
            var result1 = await _controller.CreateBooking(request1);
            var created1 = Assert.IsType<CreatedAtActionResult>(result1.Result);
            var booking1 = Assert.IsType<Booking>(created1.Value);

            // Create second booking
            var request2 = new BookingRequest
            {
                RoomId = 3,
                CustomerName = "Guest 2",
                CheckIn = DateTime.Now.AddDays(3),
                CheckOut = DateTime.Now.AddDays(4),
                NumberOfGuests = 2
            };
            var result2 = await _controller.CreateBooking(request2);
            var created2 = Assert.IsType<CreatedAtActionResult>(result2.Result);
            var booking2 = Assert.IsType<Booking>(created2.Value);

            // Assert - References should be different
            Assert.NotEqual(booking1.BookingReference, booking2.BookingReference);
            Assert.True(booking1.BookingReference.Length == 8);
            Assert.True(booking2.BookingReference.Length == 8);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
