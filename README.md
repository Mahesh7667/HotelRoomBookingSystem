# Hotel Room Booking System

A .NET 8 Web API for managing hotel room bookings, built with Entity Framework Core and SQLite.

## Features
- Manage hotels, rooms, and bookings
- Check room availability
- Create and manage bookings
- RESTful API with Swagger documentation

## Tech Stack
- **Backend:** .NET 8, ASP.NET Core Web API
- **Database:** SQLite with Entity Framework Core
- **Testing:** xUnit
- **Deployment:** Azure App Service (Free Tier)

## Live Demo
- **API Base URL:** https://hotelroombooking20250912180532-g0crcvcycjg2hbcm.canadacentral-01.azurewebsites.net
- **Swagger UI:** https://hotelroombooking20250912180532-g0crcvcycjg2hbcm.canadacentral-01.azurewebsites.net/swagger

## Local Setup
1. Clone the repo: `git clone https://github.com/Mahesh7667/HotelRoomBookingSystem.git`
2. Navigate to project: `cd HotelRoomBookingSystem`
3. Restore packages: `dotnet restore`
4. Run migrations: `dotnet ef database update`
5. Run the app: `dotnet run`
6. Access Swagger at `https://localhost:5001/swagger` (or HTTP port)

## API Endpoints
- **Hotels:** `/api/hotels` (GET, POST, PUT, DELETE)
- **Rooms:** `/api/rooms` (GET, POST, PUT, DELETE)
- **Bookings:** `/api/bookings` (GET, POST, PUT, DELETE)
- **Data:** `/api/data/seed` (POST - Seed initial data)

## Deployment to Azure
1. Use Azure CLI or VSCode extension.
2. Create App Service (F1 Free Tier).
3. Deploy via `dotnet publish` and zip upload.
4. Database persists in App Service storage.

## Testing
Run tests: `dotnet test`

## Contributing
1. Fork the repo.
2. Create a feature branch.
3. Commit changes.
4. Push and create PR.

## License
MIT
