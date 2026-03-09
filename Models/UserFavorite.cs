using System;

namespace RecetasAPINet.Models
{
    public class UserFavorite
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
    }
}
