using System;

namespace MovieManagement.Models
{
    public class WatchlistMovie
    {
        public int WatchlistId { get; set; }
        public Watchlist Watchlist { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}