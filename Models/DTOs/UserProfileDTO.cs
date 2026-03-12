namespace RecetasAPINet.DTOs
{
    public class UserProfileDTO
{
    public string Name { get; set; } = string.Empty;
    public string Surnames { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Image { get; set; }
    public string Role { get; set; } = string.Empty;
    public int RecipesCreated { get; set; }
}

}