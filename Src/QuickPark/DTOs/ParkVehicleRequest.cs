namespace QuickPark.DTOs
{
    public class ParkVehicleRequest
    {
        public int ParkingLotNumber { get; set; }

        public string LicensePlateNumber { get; set; }

        public string OwnerName { get; set; }

        public string Type { get; set; }  // Example values
    }
}
