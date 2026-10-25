using System.ComponentModel.DataAnnotations;

namespace ServerApp.DTOs;

public class CreateActionDto
{
    [Required(ErrorMessage = "Ідентифікатор користувача (UserID) є обов'язковим.")]
    public int UserID { get; set; }

    [Required(ErrorMessage = "Ідентифікатор паркувального місця (SpotID) є обов'язковим.")]
    public int SpotID { get; set; }
}