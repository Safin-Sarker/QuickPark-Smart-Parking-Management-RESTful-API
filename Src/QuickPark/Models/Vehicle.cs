namespace QuickPark.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }

        public string LicensePlateNumber { get; set; }

        public string OwnerName { get; set; }

        public string Type { get; set; }
    }
}
