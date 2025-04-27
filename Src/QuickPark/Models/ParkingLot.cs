namespace QuickPark.Models
{
    public class ParkingLot
    {
        public Guid Id { get; set; }

        public int ParkingLotNumber { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public List<ParkingSpot> Spot { get; set; }


    }
}
