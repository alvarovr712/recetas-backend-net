using System;

namespace RecetasAPINet.Models
{
    public class Session
    {
        public Guid Id { get; set; }

        // Relación con User
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public string Ip { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        public bool Enabled { get; set; }
    }
}
