using System;
using System.ComponentModel.DataAnnotations;

namespace MovieManagement.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 10)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}