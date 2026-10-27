namespace ClientApp.Models;

public class SpotItem
{
    public int ID { get; set; }
    public int Number { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? UserID { get; set; }
}