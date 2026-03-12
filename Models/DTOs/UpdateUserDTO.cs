namespace RecetasAPINet.DTOs
{
    public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Surnames { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public IFormFile? Image { get; set; }
}

}