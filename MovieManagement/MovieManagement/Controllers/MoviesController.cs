using Microsoft.AspNetCore.Mvc;
using MovieManagement.Models;
using System.Collections.Generic;

namespace MovieManagement.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            var movies = new List<Movie>
            {
                  new Movie { Id = 1, Title = "The Shawshank Redemption", Director = "Frank Darabont", Rating = 9.3, PosterUrl = "/images/shawshankredemption.jpeg" },
                  new Movie { Id = 2, Title = "The Godfather", Director = "Francis Ford Coppola", Rating = 9.2, PosterUrl = "/images/godfather.jpg" },
                  new Movie { Id = 3, Title = "The Dark Knight", Director = "Christopher Nolan", Rating = 9.0, PosterUrl = "/images/darkkinght.jpg" },
                  new Movie { Id = 4, Title = "Inception", Director = "Christopher Nolan", Rating = 9.3, PosterUrl = "/images/inception.jpg" },
                  new Movie { Id = 5, Title = "Pulp Fiction", Director = "Quentin Tarantino", Rating = 9.2, PosterUrl = "/images/pulpfiction.jpg" },
                  new Movie { Id = 6, Title = "Awake", Director = "Joby Harold", Rating = 7.0, PosterUrl = "/images/awake.jpg" }
            };
            return View(movies);
        }

        public IActionResult Details(int id)
        {
            var movie = new Movie
            {
                Id = id,
                Title = "The Shawshank Redemption",
                Director = "Frank Darabont",
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                ReleaseDate = new System.DateTime(1994, 9, 23),
                PosterUrl = "/images/shawshankredemption.jpeg",
                Rating = 9.3,
                Duration = 142,
                Categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Drama" },
                    new Category { Id = 2, Name = "Crime" }
                },
                Reviews = new List<Review>()
            };
            return View(movie);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var movie = new Movie
            {
                Id = id,
                Title = "The Shawshank Redemption",
                Director = "Frank Darabont",
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                ReleaseDate = new System.DateTime(1994, 9, 23),
                PosterUrl = "/images/shawshankredemption.jpeg",
                Rating = 9.3,
                Duration = 142
            };
            return View(movie);
        }
    }
} 