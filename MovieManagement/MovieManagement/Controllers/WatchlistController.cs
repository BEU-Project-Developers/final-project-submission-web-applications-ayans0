using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagement.Controllers
{
    [Authorize]
    public class WatchlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WatchlistController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Get user's default watchlist or create one if it doesn't exist
            var watchlist = await _context.Watchlists
                .Include(w => w.WatchlistMovies)
                .ThenInclude(wm => wm.Movie)
                .FirstOrDefaultAsync(w => w.UserId == user.Id);

            if (watchlist == null)
            {
                // Create a default watchlist for the user
                watchlist = new Watchlist
                {
                    Name = "My Watchlist",
                    UserId = user.Id
                };

                _context.Watchlists.Add(watchlist);
                await _context.SaveChangesAsync();
            }

            var movies = watchlist.WatchlistMovies
                .Select(wm => wm.Movie)
                .ToList();

            ViewData["WatchlistId"] = watchlist.Id;
            return View(movies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWatchlist(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            // Get user's default watchlist or create one if it doesn't exist
            var watchlist = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.UserId == user.Id);

            if (watchlist == null)
            {
                watchlist = new Watchlist
                {
                    Name = "My Watchlist",
                    UserId = user.Id
                };

                _context.Watchlists.Add(watchlist);
                await _context.SaveChangesAsync();
            }

            // Check if movie is already in watchlist
            var existingItem = await _context.WatchlistMovies
                .FirstOrDefaultAsync(wm => wm.WatchlistId == watchlist.Id && wm.MovieId == movieId);

            if (existingItem == null)
            {
                var watchlistMovie = new WatchlistMovie
                {
                    WatchlistId = watchlist.Id,
                    MovieId = movieId,
                    AddedAt = DateTime.Now
                };

                _context.WatchlistMovies.Add(watchlistMovie);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"'{movie.Title}' was added to your watchlist.";
            }
            else
            {
                TempData["InfoMessage"] = $"'{movie.Title}' is already in your watchlist.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWatchlist(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var watchlist = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.UserId == user.Id);

            if (watchlist == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(movieId);
            var watchlistItem = await _context.WatchlistMovies
                .FirstOrDefaultAsync(wm => wm.WatchlistId == watchlist.Id && wm.MovieId == movieId);

            if (watchlistItem != null)
            {
                _context.WatchlistMovies.Remove(watchlistItem);
                await _context.SaveChangesAsync();

                if (movie != null)
                {
                    TempData["SuccessMessage"] = $"'{movie.Title}' was removed from your watchlist.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Movie was removed from your watchlist.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateWatchlist()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWatchlist(Watchlist watchlist)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                watchlist.UserId = user.Id;
                _context.Watchlists.Add(watchlist);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "New watchlist created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(watchlist);
        }

        // Add a Search method that only searches within the user's watchlist
        public async Task<IActionResult> Search(string query)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(query))
                {
                    return RedirectToAction(nameof(Index));
                }

                // Normalize the query to lowercase for case-insensitive search
                string normalizedQuery = query.ToLower();

                // Get user's watchlist
                var watchlist = await _context.Watchlists
                    .Include(w => w.WatchlistMovies)
                    .ThenInclude(wm => wm.Movie)
                    .FirstOrDefaultAsync(w => w.UserId == user.Id);

                if (watchlist == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Filter movies in the watchlist that match the search query
                var filteredMovies = watchlist.WatchlistMovies
                    .Select(wm => wm.Movie)
                    .Where(m => m.Title.ToLower().Contains(normalizedQuery) ||
                               m.Director.ToLower().Contains(normalizedQuery) ||
                               (m.Description != null && m.Description.ToLower().Contains(normalizedQuery)))
                    .ToList();

                ViewData["SearchQuery"] = query;
                return View("Index", filteredMovies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while searching your watchlist. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}