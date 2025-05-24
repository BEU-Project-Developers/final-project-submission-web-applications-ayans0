using System;
using System.Collections.Generic;

namespace MovieManagement.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Director { get; set; }
        public string PosterUrl { get; set; }
        public int Duration { get; set; }
        public double Rating { get; set; }
        public List<Category> Categories { get; set; }
        public List<Review> Reviews { get; set; }
    }
} 