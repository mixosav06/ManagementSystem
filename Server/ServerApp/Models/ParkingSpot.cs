namespace ServerApp.Models;

public class ParkingSpot
{
    public int ID { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Status { get; set; } = "Free";

    public int? UserID { get; set; }
    public Attendant? User { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}