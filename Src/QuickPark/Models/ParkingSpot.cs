namespace QuickPark.Models
{
    public class ParkingSpot
    {
        public Guid Id { get; set; }

        public int SpotNumber { get; set; }

        public bool IsOccupied { get; set; }

        public Guid ParkingLotId { get; set; }

        public Guid? ParkedVehicleId { get; set; }

        public Vehicle? ParkedVehicle { get; set; }  


    }
}
