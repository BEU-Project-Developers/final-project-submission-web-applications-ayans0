using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManagement.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Director { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Range(0, 10)]
        public double Rating { get; set; }

        public int? Duration { get; set; }

        [MaxLength(500)]
        public string? PosterUrl { get; set; }

        [MaxLength(500)]
        public string? TrailerUrl { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
        public ICollection<WatchlistMovie> WatchlistMovies { get; set; } = new List<WatchlistMovie>();
    }
}