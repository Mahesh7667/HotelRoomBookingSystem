using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelRoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomTypesAndCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 2, 150m, "double" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 4, 250m, "deluxe" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Capacity", "HotelId", "Price", "Type" },
                values: new object[] { 1, 3, 60m, "single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Capacity", "HotelId", "Price", "Type" },
                values: new object[] { 1, 3, 60m, "single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 2, 100m, "double" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 4, 160m, "deluxe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 1, 90m, "single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 2, 150m, "double" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Capacity", "HotelId", "Price", "Type" },
                values: new object[] { 4, 2, 250m, "deluxe" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Capacity", "HotelId", "Price", "Type" },
                values: new object[] { 6, 2, 350m, "suite" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 1, 60m, "single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Capacity", "Price", "Type" },
                values: new object[] { 6, 240m, "suite" });
        }
    }
}
