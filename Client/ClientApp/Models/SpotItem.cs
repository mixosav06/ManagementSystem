using System.Text.Json.Serialization;

namespace ClientApp.Models;

public class SpotItem
{
    [JsonPropertyName("id")]
    public int ID { get; set; }

    // Ігноруємо для JSON, залишаємо лише для Binding у XAML
    [JsonIgnore]
    public int Id => ID;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}