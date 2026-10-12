namespace ServerApp.Models;

public class Attendant
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Навігаційні властивості
    public ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}