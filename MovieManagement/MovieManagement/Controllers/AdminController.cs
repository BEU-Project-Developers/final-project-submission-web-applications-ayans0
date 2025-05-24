using Microsoft.AspNetCore.Mvc;
using MovieManagement.Models;
using System.Collections.Generic;

namespace MovieManagement.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "The Shawshank Redemption", Director = "Frank Darabont", Rating = 9.3, PosterUrl = "/images/shawshankredemption.jpeg", },
                new Movie { Id = 2, Title = "The Godfather", Director = "Francis Ford Coppola", Rating = 9.2, PosterUrl = "/images/godfather.jpg" },
                new Movie { Id = 3, Title = "The Dark Knight", Director = "Christopher Nolan", Rating = 9.0, PosterUrl = "/images/darkkinght.jpg" }
            };
            return View(movies);
        }

        public IActionResult AddMovie()
        {
            return View();
        }

        public IActionResult EditMovie(int id)
        {
            var movie = new Movie
            {
                Id = id,
                Title = "The Shawshank Redemption",
                Director = "Frank Darabont",
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                ReleaseDate = new System.DateTime(1994, 9, 23),
                Rating = 9.3,
                Duration = 142,
                PosterUrl = "/images/shawshankredemption.jpeg"
            };
            return View(movie);
        }

        public IActionResult DeleteMovie(int id)
        {
            var movie = new Movie
            {
                Id = id,
                Title = "The Shawshank Redemption",
                Director = "Frank Darabont"
            };
            return View(movie);
        }
    }
} 