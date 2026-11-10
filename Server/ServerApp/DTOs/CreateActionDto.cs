using System.ComponentModel.DataAnnotations;

namespace ServerApp.DTOs;

public class CreateActionDto
{
    // UserID береться з JWT-токена, а не з тіла запиту.
    [Range(1, int.MaxValue, ErrorMessage = "Ідентифікатор паркувального місця (SpotID) є обов'язковим.")]
    public int SpotID { get; set; }
}
