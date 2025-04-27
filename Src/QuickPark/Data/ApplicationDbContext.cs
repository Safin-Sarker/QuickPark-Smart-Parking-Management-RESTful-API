using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuickPark.Models;
using System.Data.Common;
using System.Reflection.Emit;

namespace QuickPark.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ParkingLot> ParkingLots { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var lot1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var lot2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var vehicle1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var vehicle2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

            builder.Entity<ParkingLot>().HasData(
                new ParkingLot
                {
                    Id = lot1Id,
                    ParkingLotNumber = 1,
                    Name = "Main Street Lot",
                    Address = "123 Main St"
                },
                new ParkingLot
                {
                    Id = lot2Id,
                    ParkingLotNumber = 2,
                    Name = "Downtown Garage",
                    Address = "456 Downtown Blvd"
                }
            );

            builder.Entity<Vehicle>().HasData(
                new Vehicle
                {
                    Id = vehicle1Id,
                    LicensePlateNumber = "ABC-1234",
                    OwnerName = "John Doe",
                    Type = "Car"
                },
                new Vehicle
                {
                    Id = vehicle2Id,
                    LicensePlateNumber = "XYZ-5678",
                    OwnerName = "Jane Smith",
                    Type = "Truck"
                }
            );

            builder.Entity<ParkingSpot>().HasData(
                new ParkingSpot
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    SpotNumber = 1,
                    IsOccupied = true,
                    ParkingLotId = lot1Id,
                    ParkedVehicleId = vehicle1Id
                },
                new ParkingSpot
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    SpotNumber = 2,
                    IsOccupied = false,
                    ParkingLotId = lot1Id,
                    ParkedVehicleId = null
                },
                new ParkingSpot
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    SpotNumber = 1,
                    IsOccupied = true,
                    ParkingLotId = lot2Id,
                    ParkedVehicleId = vehicle2Id
                }
            );
        }

    }
}
