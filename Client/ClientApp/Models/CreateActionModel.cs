namespace ClientApp.Models;

// UserID не передаємо: сервер бере його з JWT-токена.
public class CreateActionModel
{
    public int SpotID { get; set; }
}
