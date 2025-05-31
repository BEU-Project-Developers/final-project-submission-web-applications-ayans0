using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManagement.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }
}