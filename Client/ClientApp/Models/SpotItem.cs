namespace ClientApp.Models;

public class SpotItem
{
    public int ID { get; set; }
    public string Number { get; set; } = string.Empty; // на сервері номер рядковий: "001", "103"
    public string Status { get; set; } = string.Empty;
    public int? UserID { get; set; }
}
