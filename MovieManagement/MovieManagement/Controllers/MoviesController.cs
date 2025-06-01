using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagement.Models;
using MovieManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagement.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;

        public MoviesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var movies = await _context.Movies
                    .OrderByDescending(m => m.Rating)
                    .ToListAsync();

                return View(movies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading movies. Please try again later.";
                return View(new List<Movie>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieCategories)
                    .ThenInclude(mc => mc.Category)
                    .Include(m => m.Reviews)
                    .ThenInclude(r => r.User)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    TempData["ErrorMessage"] = "Movie not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(movie);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the movie details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(int movieId, int rating, string comment)
        {
            try
            {
                if (rating < 1 || rating > 10)
                {
                    TempData["ErrorMessage"] = "Rating must be between 1 and 10.";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                var movie = await _context.Movies.FindAsync(movieId);
                if (movie == null)
                {
                    TempData["ErrorMessage"] = "Movie not found.";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                // Check if user already reviewed this movie
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == user.Id);

                if (existingReview != null)
                {
                    TempData["ErrorMessage"] = "You have already reviewed this movie. You can only submit one review per movie.";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                if (string.IsNullOrWhiteSpace(comment))
                {
                    comment = string.Empty;
                }
                else if (comment.Length > 1000)
                {
                    TempData["ErrorMessage"] = "Review comment cannot exceed 1000 characters.";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                var review = new Review
                {
                    MovieId = movieId,
                    UserId = user.Id,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.Now
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Update movie average rating
                var movieReviews = await _context.Reviews
                    .Where(r => r.MovieId == movieId)
                    .ToListAsync();

                if (movieReviews.Any())
                {
                    movie.Rating = Math.Round(movieReviews.Average(r => r.Rating), 1);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Your review has been added successfully.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding your review. Please try again later.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId, int movieId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Check if the review belongs to the user or if the user is an admin
                if (review.UserId != user.Id && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You do not have permission to delete this review.";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                // Update movie average rating
                var movie = await _context.Movies.FindAsync(movieId);
                if (movie != null)
                {
                    var movieReviews = await _context.Reviews
                        .Where(r => r.MovieId == movieId)
                        .ToListAsync();

                    if (movieReviews.Any())
                    {
                        movie.Rating = Math.Round(movieReviews.Average(r => r.Rating), 1);
                    }
                    else
                    {
                        movie.Rating = 0;
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Review has been deleted successfully.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the review. Please try again later.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
        }

        [Authorize]
        public async Task<IActionResult> AddToWatchlist(int movieId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                var movie = await _context.Movies.FindAsync(movieId);
                if (movie == null)
                {
                    TempData["ErrorMessage"] = "Movie not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get or create default watchlist
                var watchlist = await _context.Watchlists
                    .FirstOrDefaultAsync(w => w.UserId == user.Id && w.Name == "My Watchlist");

                if (watchlist == null)
                {
                    watchlist = new Watchlist
                    {
                        UserId = user.Id,
                        Name = "My Watchlist",
                        CreatedAt = DateTime.Now
                    };

                    _context.Watchlists.Add(watchlist);
                    await _context.SaveChangesAsync();
                }

                // Check if movie is already in watchlist
                var existingEntry = await _context.WatchlistMovies
                    .FirstOrDefaultAsync(wm => wm.WatchlistId == watchlist.Id && wm.MovieId == movieId);

                if (existingEntry != null)
                {
                    TempData["ErrorMessage"] = "This movie is already in your watchlist.";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Add movie to watchlist
                var watchlistMovie = new WatchlistMovie
                {
                    WatchlistId = watchlist.Id,
                    MovieId = movieId,
                    AddedAt = DateTime.Now
                };

                _context.WatchlistMovies.Add(watchlistMovie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Movie has been added to your watchlist.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the movie to your watchlist. Please try again later.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromWatchlist(int movieId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                // Get default watchlist
                var watchlist = await _context.Watchlists
                    .FirstOrDefaultAsync(w => w.UserId == user.Id && w.Name == "My Watchlist");

                if (watchlist == null)
                {
                    TempData["ErrorMessage"] = "Watchlist not found.";
                    return RedirectToAction(nameof(Watchlist));
                }

                // Find watchlist entry
                var watchlistMovie = await _context.WatchlistMovies
                    .FirstOrDefaultAsync(wm => wm.WatchlistId == watchlist.Id && wm.MovieId == movieId);

                if (watchlistMovie == null)
                {
                    TempData["ErrorMessage"] = "Movie not found in your watchlist.";
                    return RedirectToAction(nameof(Watchlist));
                }

                _context.WatchlistMovies.Remove(watchlistMovie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Movie has been removed from your watchlist.";
                return RedirectToAction(nameof(Watchlist));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while removing the movie from your watchlist. Please try again later.";
                return RedirectToAction(nameof(Watchlist));
            }
        }

        public async Task<IActionResult> Search(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return RedirectToAction(nameof(Index));
                }

                // Normalize the query to lowercase for case-insensitive search
                string normalizedQuery = query.ToLower();

                var movies = await _context.Movies
                    .Where(m => m.Title.ToLower().Contains(normalizedQuery) ||
                                 m.Director.ToLower().Contains(normalizedQuery) ||
                                 (m.Description != null && m.Description.ToLower().Contains(normalizedQuery)))
                    .ToListAsync();

                ViewData["SearchQuery"] = query;
                return View("Index", movies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while searching for movies. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Category(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.MovieCategories)
                    .ThenInclude(mc => mc.Movie)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                var movies = category.MovieCategories.Select(mc => mc.Movie).ToList();
                ViewData["CategoryName"] = category.Name;

                return View("Index", movies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading movies by category. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}