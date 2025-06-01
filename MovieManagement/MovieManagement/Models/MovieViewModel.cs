using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MovieManagement.Models
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Director")]
        [MaxLength(100)]
        public string Director { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [MaxLength(2000)]
        public string? Description { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Rating")]
        [Range(0, 10)]
        public double Rating { get; set; }

        [Display(Name = "Duration (minutes)")]
        public int? Duration { get; set; }

        [Display(Name = "Poster")]
        public IFormFile? PosterFile { get; set; }

        [Display(Name = "Current Poster")]
        public string? PosterUrl { get; set; }

        [Display(Name = "Trailer URL (YouTube, Vimeo, etc.)")]
        [MaxLength(500)]
        public string? TrailerUrl { get; set; }

        [Display(Name = "Categories")]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public List<Category>? AvailableCategories { get; set; }
    }
}