using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagement.Models
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed roles
            await SeedRoles(roleManager);

            // Seed users
            await SeedUsers(userManager, roleManager);

            // Seed categories
            await SeedCategories(context);

            // Seed movies
            await SeedMovies(context);

            // Save all changes
            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Check if admin already exists
            var adminEmail = "admin@movies.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create a new admin user if none exists
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (!result.Succeeded)
                {
                    // Log error but continue
                    Console.WriteLine($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Ensure admin has Admin role
            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    // Log error but continue
                    Console.WriteLine($"Failed to add admin to role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }
            }

            // Check if regular user exists
            var userEmail = "user@movies.com";
            var regularUser = await userManager.FindByEmailAsync(userEmail);

            if (regularUser == null)
            {
                // Create a new regular user if none exists
                regularUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "Regular",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(regularUser, "User123!");
                if (!result.Succeeded)
                {
                    // Log error but continue
                    Console.WriteLine($"Failed to create regular user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Ensure regular user has User role
            if (regularUser != null && !await userManager.IsInRoleAsync(regularUser, "User"))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(regularUser, "User");
                if (!addToRoleResult.Succeeded)
                {
                    // Log error but continue
                    Console.WriteLine($"Failed to add user to role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        private static async Task SeedCategories(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Action" },
                    new Category { Name = "Adventure" },
                    new Category { Name = "Comedy" },
                    new Category { Name = "Drama" },
                    new Category { Name = "Horror" },
                    new Category { Name = "Sci-Fi" },
                    new Category { Name = "Thriller" },
                    new Category { Name = "Romance" },
                    new Category { Name = "Fantasy" },
                    new Category { Name = "Animation" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedMovies(ApplicationDbContext context)
        {
            if (!context.Movies.Any())
            {
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "The Shawshank Redemption",
                        Director = "Frank Darabont",
                        Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                        ReleaseDate = new DateTime(1994, 9, 23),
                        Rating = 9.3,
                        Duration = 142,
                        PosterUrl = "/images/shawshankredemption.jpeg",
                        TrailerUrl = "https://youtu.be/6hB3S9bIaco"
                    },
                        new Movie
                    {
                        Title = "The Revenant",
                        Director = "Dan Reynolds",
                        Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                        ReleaseDate = new DateTime(1994, 9, 23),
                        Rating = 8.4,
                        Duration = 142,
                        PosterUrl = "/images/shawshankredemption.jpeg",
                        TrailerUrl = "https://youtu.be/6hB3S9bIaco"
                    },
                    new Movie
                    {
                        Title = "The Godfather",
                        Director = "Francis Ford Coppola",
                        Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                        ReleaseDate = new DateTime(1972, 3, 24),
                        Rating = 9.2,
                        Duration = 175,
                        PosterUrl = "/images/godfather.jpg",
                        TrailerUrl = "https://youtu.be/sY1S34973zA"
                    },
                    new Movie
                    {
                        Title = "The Dark Knight",
                        Director = "Christopher Nolan",
                        Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                        ReleaseDate = new DateTime(2008, 7, 18),
                        Rating = 9.0,
                        Duration = 152,
                        PosterUrl = "/images/darkkinght.jpg",
                        TrailerUrl = "https://youtu.be/EXeTwQWrcwY"
                    },
                    new Movie
                    {
                        Title = "Inception",
                        Director = "Christopher Nolan",
                        Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                        ReleaseDate = new DateTime(2010, 7, 16),
                        Rating = 8.8,
                        Duration = 148,
                        PosterUrl = "/images/inception.jpg",
                        TrailerUrl = "https://youtu.be/YoHD9XEInc0"
                    },
                    new Movie
                    {
                        Title = "Pulp Fiction",
                        Director = "Quentin Tarantino",
                        Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                        ReleaseDate = new DateTime(1994, 10, 14),
                        Rating = 8.9,
                        Duration = 154,
                        PosterUrl = "/images/pulpfiction.jpg",
                        TrailerUrl = "https://youtu.be/s7EdQ4FqbhY"
                    },
                    new Movie
                    {
                        Title = "Awake",
                        Director = "Joby Harold",
                        Description = "A wealthy, successful man undergoes heart surgery and becomes aware during the operation, hearing the surgeon's plan to kill him.",
                        ReleaseDate = new DateTime(2007, 11, 30),
                        Rating = 6.5,
                        Duration = 84,
                        PosterUrl = "/images/awake.jpg",
                        TrailerUrl = "https://youtu.be/k42HgNZemCQ"
                    }
                };

                context.Movies.AddRange(movies);
                await context.SaveChangesAsync();

                // Add movie categories
                var dramaCategory = context.Categories.FirstOrDefault(c => c.Name == "Drama");
                var actionCategory = context.Categories.FirstOrDefault(c => c.Name == "Action");
                var thrillerCategory = context.Categories.FirstOrDefault(c => c.Name == "Thriller");
                var crimeCategory = context.Categories.FirstOrDefault(c => c.Name == "Thriller");
                var sciFiCategory = context.Categories.FirstOrDefault(c => c.Name == "Sci-Fi");

                if (dramaCategory != null && crimeCategory != null)
                {
                    // Shawshank Redemption categories
                    var shawshank = context.Movies.FirstOrDefault(m => m.Title == "The Shawshank Redemption");
                    if (shawshank != null)
                    {
                        context.Add(new MovieCategory { MovieId = shawshank.Id, CategoryId = dramaCategory.Id });
                        context.Add(new MovieCategory { MovieId = shawshank.Id, CategoryId = crimeCategory.Id });
                    }

                    // Godfather categories
                    var godfather = context.Movies.FirstOrDefault(m => m.Title == "The Godfather");
                    if (godfather != null)
                    {
                        context.Add(new MovieCategory { MovieId = godfather.Id, CategoryId = dramaCategory.Id });
                        context.Add(new MovieCategory { MovieId = godfather.Id, CategoryId = crimeCategory.Id });
                    }
                }

                if (actionCategory != null && thrillerCategory != null)
                {
                    // Dark Knight categories
                    var darkKnight = context.Movies.FirstOrDefault(m => m.Title == "The Dark Knight");
                    if (darkKnight != null)
                    {
                        context.Add(new MovieCategory { MovieId = darkKnight.Id, CategoryId = actionCategory.Id });
                        context.Add(new MovieCategory { MovieId = darkKnight.Id, CategoryId = thrillerCategory.Id });
                    }
                }

                if (sciFiCategory != null && actionCategory != null)
                {
                    // Inception categories
                    var inception = context.Movies.FirstOrDefault(m => m.Title == "Inception");
                    if (inception != null)
                    {
                        context.Add(new MovieCategory { MovieId = inception.Id, CategoryId = sciFiCategory.Id });
                        context.Add(new MovieCategory { MovieId = inception.Id, CategoryId = actionCategory.Id });
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}