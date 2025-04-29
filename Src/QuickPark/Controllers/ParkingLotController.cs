using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickPark.Data;
using QuickPark.Models;

namespace QuickPark.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParkingLotController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingLotController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
             var lots = await _context.ParkingLots.Include(x=> x.Spot).ToListAsync();
            return Ok(lots);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var lot = await _context.ParkingLots.Include(x => x.Spot).FirstOrDefaultAsync(x=>x.Id==id);

            return Ok(lot);
        }


        [HttpPost]
        public async Task<IActionResult> Create (ParkingLot parkingLot)
        {
            var exist=await _context.ParkingLots.AnyAsync(x=>x.ParkingLotNumber==parkingLot.ParkingLotNumber);

            if (exist) 
            { 
                return BadRequest($"parking lot Number{parkingLot.ParkingLotNumber} is already exist");
            }

            parkingLot.Id=Guid.NewGuid();

            if (parkingLot.Spot!= null && parkingLot.Spot.Any())
            {
                foreach(var spot in parkingLot.Spot)
                {
                    spot.Id = Guid.NewGuid(); 
                    spot.ParkingLotId = parkingLot.Id; 
                    spot.IsOccupied = false; 
                    spot.ParkedVehicleId = null; 

                }
            }


            await _context.ParkingLots.AddAsync(parkingLot);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new {id= parkingLot.Id}, parkingLot);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ParkingLot updatedLot)
        {
            if (id != updatedLot.Id)
            {
                return BadRequest("Id in URL and body must match.");
            }

            var existingLot = await _context.ParkingLots
                .Include(x => x.Spot) 
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingLot == null)
            {
                return NotFound();
            }

            // Update ParkingLot fields
            existingLot.Name = updatedLot.Name;
            existingLot.Address = updatedLot.Address;

            // --- Now update Spots if provided ---
            if (updatedLot.Spot != null)
            {
                foreach (var updatedSpot in updatedLot.Spot)
                {
                    var existingSpot = existingLot.Spot.FirstOrDefault(s => s.Id == updatedSpot.Id);
                    if (existingSpot != null)
                    {
                        existingSpot.IsOccupied = updatedSpot.IsOccupied;
                        existingSpot.ParkedVehicleId = updatedSpot.ParkedVehicleId;
                    }
                    else
                    {
                        // If spot doesn't exist yet (new), you can add it
                        updatedSpot.Id = Guid.NewGuid(); // assign new ID
                        updatedSpot.ParkingLotId = existingLot.Id; // link it to the lot
                        existingLot.Spot.Add(updatedSpot);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return NoContent(); 
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingLot = await _context.ParkingLots
                .Include(x => x.Spot)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingLot == null)
            {
                return NotFound("Parking lot not found.");
            }

            // First, remove all spots related to this parking lot
            if (existingLot.Spot != null && existingLot.Spot.Any())
            {
                foreach (var spot in existingLot.Spot)
                {
                    // Optional: also remove parked vehicles if needed
                    if (spot.ParkedVehicleId != null)
                    {
                        var vehicle = await _context.Vehicles.FindAsync(spot.ParkedVehicleId);
                        if (vehicle != null)
                        {
                             _context.Vehicles.Remove(vehicle);
                        }
                    }
                    _context.ParkingSpots.Remove(spot);
                }
            }

            _context.ParkingLots.Remove(existingLot);

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Parking lot {existingLot.ParkingLotNumber} deleted successfully." });
        }



    }
}
