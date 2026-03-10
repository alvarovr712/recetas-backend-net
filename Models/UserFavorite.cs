using System;

namespace RecetasAPINet.Models
{
    public class UserFavorite
    {
        public Guid UserId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public User? User { get; set; }

        public Guid RecipeId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Recipe? Recipe { get; set; }
    }
}
