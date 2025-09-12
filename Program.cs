using HotelRoomBooking.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().
   AddJsonOptions(options => {
       options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HotelBookingContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories and services
builder.Services.AddScoped<HotelRoomBooking.Repositories.IUnitOfWork, HotelRoomBooking.Repositories.UnitOfWork>();
builder.Services.AddScoped<HotelRoomBooking.Services.IBookingService, HotelRoomBooking.Services.BookingService>();
builder.Services.AddScoped<HotelRoomBooking.Services.IRoomService, HotelRoomBooking.Services.RoomService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
