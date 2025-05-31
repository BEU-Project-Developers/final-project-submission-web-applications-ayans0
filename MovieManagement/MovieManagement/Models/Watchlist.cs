using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManagement.Models
{
    public class Watchlist
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<WatchlistMovie> WatchlistMovies { get; set; } = new List<WatchlistMovie>();
    }
}