using Microsoft.AspNetCore.Mvc;
using MovieManagement.Models;
using System.Collections.Generic;

namespace MovieManagement.Controllers
{
    public class WatchlistController : Controller
    {
        public IActionResult Index()
        {
            var watchlist = new List<Movie>
            {
                new Movie { Id = 1, Title = "The Shawshank Redemption", Director = "Frank Darabont", Rating = 9.3, PosterUrl = "/images/shawshankredemption.jpeg"  },
                new Movie { Id = 2, Title = "The Godfather", Director = "Francis Ford Coppola", Rating = 9.2, PosterUrl = "/images/godfather.jpg" },
                  new Movie { Id = 4, Title = "Inception", Director = "Christopher Nolan", Rating = 9.3, PosterUrl = "/images/inception.jpg" },
                  new Movie { Id = 5, Title = "Pulp Fiction", Director = "Quentin Tarantino", Rating = 9.2, PosterUrl = "/images/pulpfiction.jpg" }
            };
            return View(watchlist);
        }

        public IActionResult AddToWatchlist(int movieId)
        {
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromWatchlist(int movieId)
        {
            return RedirectToAction("Index");
        }
    }
} 