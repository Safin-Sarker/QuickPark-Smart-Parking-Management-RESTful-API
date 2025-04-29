namespace QuickPark.DTOs
{
    public class UnparkVehicleRequest
    {
        public int ParkingLotNumber { get; set; } // 🔥 ParkingLotNumber instead of Id
        public int SpotNumber { get; set; }
    }
}
