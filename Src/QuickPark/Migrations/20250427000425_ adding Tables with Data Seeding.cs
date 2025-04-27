using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuickPark.Migrations
{
    /// <inheritdoc />
    public partial class addingTableswithDataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParkingLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParkingLotNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingLots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicensePlateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSpots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpotNumber = table.Column<int>(type: "int", nullable: false),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false),
                    ParkingLotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParkedVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSpots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingSpots_ParkingLots_ParkingLotId",
                        column: x => x.ParkingLotId,
                        principalTable: "ParkingLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParkingSpots_Vehicles_ParkedVehicleId",
                        column: x => x.ParkedVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ParkingLots",
                columns: new[] { "Id", "Address", "Name", "ParkingLotNumber" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "123 Main St", "Main Street Lot", 1 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "456 Downtown Blvd", "Downtown Garage", 2 }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "LicensePlateNumber", "OwnerName", "Type" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "ABC-1234", "John Doe", "Car" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "XYZ-5678", "Jane Smith", "Truck" }
                });

            migrationBuilder.InsertData(
                table: "ParkingSpots",
                columns: new[] { "Id", "IsOccupied", "ParkedVehicleId", "ParkingLotId", "SpotNumber" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), true, new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), 1 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), false, null, new Guid("11111111-1111-1111-1111-111111111111"), 2 },
                    { new Guid("77777777-7777-7777-7777-777777777777"), true, new Guid("44444444-4444-4444-4444-444444444444"), new Guid("22222222-2222-2222-2222-222222222222"), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_ParkedVehicleId",
                table: "ParkingSpots",
                column: "ParkedVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_ParkingLotId",
                table: "ParkingSpots",
                column: "ParkingLotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingSpots");

            migrationBuilder.DropTable(
                name: "ParkingLots");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
