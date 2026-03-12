using RecetasAPINet.Enums;

namespace RecetasAPINet.DTOs
{
    public class TokenInfoDTO
    {
        public int UserId {get; set;}
        public string Username {get; set;} = string.Empty;
        public Role Role {get;set;}
        public string Image { get; set; } = string.Empty;
        public DateTime Expires {get;set;}
    }
}