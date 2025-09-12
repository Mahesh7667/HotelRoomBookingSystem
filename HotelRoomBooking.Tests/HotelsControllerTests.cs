using HotelRoomBooking.Controllers;
using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using HotelRoomBooking.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelRoomBooking.Tests
{
    public class HotelsControllerTests : IDisposable
    {
        private readonly HotelBookingContext _context;
        private readonly HotelsController _controller;

        public HotelsControllerTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<HotelBookingContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotelBookingContext(options);
            _controller = new HotelsController(new UnitOfWork(_context));

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
                new Room { Id = 1, Type = "single", Capacity = 1, Price = 60, HotelId = 1 },
                new Room { Id = 2, Type = "single", Capacity = 1, Price = 60, HotelId = 1 },
                new Room { Id = 3, Type = "double", Capacity = 2, Price = 100, HotelId = 1 },
                new Room { Id = 4, Type = "double", Capacity = 2, Price = 100, HotelId = 1 },
                new Room { Id = 5, Type = "deluxe", Capacity = 4, Price = 160, HotelId = 1 },
                new Room { Id = 6, Type = "deluxe", Capacity = 4, Price = 160, HotelId = 1 }
            };
            _context.Rooms.AddRange(rooms);

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetHotelByName_ExistingHotel_ReturnsHotel()
        {
            // Act
            var actionResult = await _controller.GetHotelByName("Test Hotel");
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var hotel = Assert.IsType<Hotel>(okResult.Value);
            Assert.Equal("Test Hotel", hotel.Name);
            Assert.Equal(6, hotel.Rooms.Count); // Now 6 rooms per hotel
        }

        [Fact]
        public async Task GetHotelByName_NonExistingHotel_ReturnsNotFound()
        {
            // Act
            var actionResult = await _controller.GetHotelByName("Non Existing Hotel");
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetHotelByName_EmptyName_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetHotelByName("");
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAllHotels_ReturnsAllHotels()
        {
            // Act
            var actionResult = await _controller.GetAllHotels();
            var result = actionResult.Result; // Unwrap the ActionResult

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var hotels = Assert.IsType<List<Hotel>>(okResult.Value);
            Assert.Single(hotels);
            Assert.Equal("Test Hotel", hotels.First().Name);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
