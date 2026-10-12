namespace ServerApp.Models;

public class Reservation
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public Attendant? User { get; set; }

    public int SpotID { get; set; }
    public ParkingSpot? Spot { get; set; }

    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
}