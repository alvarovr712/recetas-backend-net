using RecetasAPINet.Enums;

namespace RecetasAPINet.DTOs
{
    public class TokenInfoDTO
    {
        public string UserId {get; set;} = string.Empty;
        public string Username {get; set;} = string.Empty;
        public Role Role {get;set;}
        public string Image { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime Expires {get;set;}
    }
}