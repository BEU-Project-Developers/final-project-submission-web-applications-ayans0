using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieManagement.Models;
using MovieManagement.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public AdminController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            try
            { //filmleri dbdan goturur
                var movies = await _context.Movies.ToListAsync();
                return View(movies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading movies. Please try again later.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> AddMovie()
        {
            try
            {
                var viewModel = new MovieViewModel
                {
                    AvailableCategories = await _context.Categories.ToListAsync()
                };

                return View(viewModel); //film elave etmek ucun formu acir
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while preparing to add a movie. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie(MovieViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var movie = new Movie
                    {
                        Title = viewModel.Title,
                        Director = viewModel.Director,
                        Description = viewModel.Description,
                        ReleaseDate = viewModel.ReleaseDate,
                        Rating = viewModel.Rating,
                        Duration = viewModel.Duration,
                        TrailerUrl = viewModel.TrailerUrl
                    };

                    // Handle poster file upload
                    if (viewModel.PosterFile != null)
                    {
                        // Validate file type
                        string fileExtension = Path.GetExtension(viewModel.PosterFile.FileName).ToLowerInvariant();
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                        if (!Array.Exists(allowedExtensions, ext => ext.Equals(fileExtension)))
                        {
                            ModelState.AddModelError("PosterFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed");
                            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                            return View(viewModel);
                        }

                        // Validate file size (max 5MB)
                        if (viewModel.PosterFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("PosterFile", "File size should not exceed 5MB");
                            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                            return View(viewModel);
                        }

                        movie.PosterUrl = await _fileService.UploadFileAsync(viewModel.PosterFile, "images");
                    }

                    _context.Movies.Add(movie);
                    await _context.SaveChangesAsync();

                    // Add movie categories
                    if (viewModel.SelectedCategoryIds != null && viewModel.SelectedCategoryIds.Any())
                    {
                        foreach (var categoryId in viewModel.SelectedCategoryIds)
                        {
                            _context.Add(new MovieCategory
                            {
                                MovieId = movie.Id,
                                CategoryId = categoryId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Movie was successfully added.";
                    return RedirectToAction(nameof(Index));
                }

                // If we reach here, something failed; redisplay form
                viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while adding the movie. Please try again.");
                viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                return View(viewModel);
            }
        }

        public async Task<IActionResult> EditMovie(int id)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieCategories)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    TempData["ErrorMessage"] = "Movie not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new MovieViewModel
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Director = movie.Director,
                    Description = movie.Description,
                    ReleaseDate = movie.ReleaseDate,
                    Rating = movie.Rating,
                    Duration = movie.Duration,
                    TrailerUrl = movie.TrailerUrl,
                    PosterUrl = movie.PosterUrl,
                    SelectedCategoryIds = movie.MovieCategories.Select(mc => mc.CategoryId).ToList(),
                    AvailableCategories = await _context.Categories.ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the movie for editing. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(MovieViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var movie = await _context.Movies.FindAsync(viewModel.Id);
                    if (movie == null)
                    {
                        TempData["ErrorMessage"] = "Movie not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    //melumatlari yenileyir
                    movie.Title = viewModel.Title;
                    movie.Director = viewModel.Director;
                    movie.Description = viewModel.Description;
                    movie.ReleaseDate = viewModel.ReleaseDate;
                    movie.Rating = viewModel.Rating;
                    movie.Duration = viewModel.Duration;
                    movie.TrailerUrl = viewModel.TrailerUrl;

                    // Handle poster file upload
                    if (viewModel.PosterFile != null)
                    {
                        // Validate file type
                        string fileExtension = Path.GetExtension(viewModel.PosterFile.FileName).ToLowerInvariant();
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                        if (!Array.Exists(allowedExtensions, ext => ext.Equals(fileExtension)))
                        {
                            ModelState.AddModelError("PosterFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed");
                            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                            return View(viewModel);
                        }

                        // Validate file size (max 5MB)
                        if (viewModel.PosterFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("PosterFile", "File size should not exceed 5MB");
                            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                            return View(viewModel);
                        }

                        // Delete old poster if exists
                        if (!string.IsNullOrEmpty(movie.PosterUrl))
                        {
                            _fileService.DeleteFile(movie.PosterUrl);
                        }

                        movie.PosterUrl = await _fileService.UploadFileAsync(viewModel.PosterFile, "images");
                    }

                    // Update movie categories
                    // First remove all existing categories
                    var existingCategories = await _context.MovieCategories
                        .Where(mc => mc.MovieId == movie.Id)
                        .ToListAsync();

                    _context.MovieCategories.RemoveRange(existingCategories);

                    // Then add selected categories
                    if (viewModel.SelectedCategoryIds != null && viewModel.SelectedCategoryIds.Any())
                    {
                        foreach (var categoryId in viewModel.SelectedCategoryIds)
                        {
                            _context.MovieCategories.Add(new MovieCategory
                            {
                                MovieId = movie.Id,
                                CategoryId = categoryId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Movie was successfully updated.";
                    return RedirectToAction(nameof(Index));
                }

                viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the movie. Please try again.");
                viewModel.AvailableCategories = await _context.Categories.ToListAsync();
                return View(viewModel);
            }
        }

        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieCategories)
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
                TempData["ErrorMessage"] = "An error occurred while loading the movie for deletion. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("DeleteMovie")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovieConfirmed(int id)
        {
            try
            { //filme aid melumatlari alir
                var movie = await _context.Movies
                    .Include(m => m.MovieCategories)
                    .Include(m => m.Reviews)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    TempData["ErrorMessage"] = "Movie not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete poster file if exists
                if (!string.IsNullOrEmpty(movie.PosterUrl))
                {
                    try
                    {
                        _fileService.DeleteFile(movie.PosterUrl);
                    }
                    catch
                    {
                        // Continue with movie deletion even if file deletion fails
                    }
                }

                // Entity Framework will handle deleting related entities based on cascade delete 
                //(moviecategory ve reviewler de avtomatik silinir)
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Movie was successfully deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the movie. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}