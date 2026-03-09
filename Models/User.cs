using System;
using System.Collections.Generic;
using RecetasAPINet.Enums;

namespace RecetasAPINet.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Surnames { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public string Password {get; set;} = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string? Image { get; set; } = string.Empty;
        public Role Role { get; set; }
        public bool Enabled { get; set; }

        // Relaciones
        public List<Recipe> Recipes { get; set; } = new();
        public List<UserFavorite> Favorites { get; set; } = new();
        public List<Session> Sessions { get; set; } = new();
    }
}
