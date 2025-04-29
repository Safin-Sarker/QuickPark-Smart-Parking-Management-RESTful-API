using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickPark.Data;
using QuickPark.DTOs;
using QuickPark.Models;

namespace QuickPark.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        [HttpGet("status/{parkingLotId}")]
        public async Task<IActionResult> GetParkingStatus(Guid parkingLotId)
        {
            var lot = await _context.ParkingLots
                .Include(x => x.Spot)
                    .ThenInclude(s => s.ParkedVehicle)
                .FirstOrDefaultAsync(x => x.Id == parkingLotId);

            if (lot == null)
            {
                return NotFound();
            }

            var status = lot.Spot.Select(s => new
            {
                SpotNumber = s.SpotNumber,
                IsOccupied = s.IsOccupied,
                Vehicle = s.ParkedVehicle != null ? new
                {
                    s.ParkedVehicle.LicensePlateNumber,
                    s.ParkedVehicle.OwnerName,
                    s.ParkedVehicle.Type
                } : null
            });

            return Ok(new
            {
                ParkingLotName = lot.Name,
                Address = lot.Address,
                ParkingStatus = status
            });
        }

        [HttpPost("park")]
        public async Task<IActionResult> ParkVehicle(ParkVehicleRequest request)
        {
            var lot = await _context.ParkingLots
                .Include(x => x.Spot)
                    .ThenInclude(s => s.ParkedVehicle) // Include ParkedVehicle
                .FirstOrDefaultAsync(x => x.ParkingLotNumber == request.ParkingLotNumber);

            if (lot == null)
                return NotFound("Parking lot not found.");

            var freeSpot = lot.Spot.FirstOrDefault(s => !s.IsOccupied);

            if (freeSpot == null)
                return BadRequest("No free spots available.");

            // Check if the same vehicle is already parked somewhere
            var vehicleAlreadyParked = await _context.ParkingSpots
                .Include(s => s.ParkedVehicle)
                .AnyAsync(s => s.ParkedVehicle != null && s.ParkedVehicle.LicensePlateNumber == request.LicensePlateNumber);

            if (vehicleAlreadyParked)
            {
                return BadRequest("This vehicle is already parked in a spot.");
            }

            // Create new Vehicle
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                LicensePlateNumber = request.LicensePlateNumber,
                OwnerName = request.OwnerName,
                Type = request.Type
            };

            _context.Vehicles.Add(vehicle);

            // Assign Vehicle to ParkingSpot
            freeSpot.IsOccupied = true;
            freeSpot.ParkedVehicleId = vehicle.Id;

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Vehicle parked at Spot {freeSpot.SpotNumber} in Parking Lot {lot.ParkingLotNumber}." });
        }

        [HttpPost("unpark")]
        public async Task<IActionResult> UnparkVehicle(UnparkVehicleRequest request)
        {
            var lot = await _context.ParkingLots
                .Include(x => x.Spot)
                    .ThenInclude(s => s.ParkedVehicle)
                .FirstOrDefaultAsync(x => x.ParkingLotNumber == request.ParkingLotNumber);

            if (lot == null)
                return NotFound("Parking lot not found.");

            var spot = lot.Spot.FirstOrDefault(s => s.SpotNumber == request.SpotNumber);

            if (spot == null)
                return NotFound($"Spot number {request.SpotNumber} not found in this lot.");

            if (!spot.IsOccupied)
                return BadRequest("This spot is already free.");

            if (spot.ParkedVehicleId != null)
            {
                var vehicle = await _context.Vehicles.FindAsync(spot.ParkedVehicleId);
                if (vehicle != null)
                {
                    _context.Vehicles.Remove(vehicle); 
                }
            }

            // Unassign the parking spot
            spot.IsOccupied = false;
            spot.ParkedVehicleId = null;

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Vehicle removed and Spot {spot.SpotNumber} in Parking Lot {lot.ParkingLotNumber} is now free." });
        }



    }
}
