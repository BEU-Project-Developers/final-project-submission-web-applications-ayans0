using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MovieManagement.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<WatchlistMovie> WatchlistMovies { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieCategory>()
                .HasKey(mc => new { mc.MovieId, mc.CategoryId });

            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieCategories)
                .HasForeignKey(mc => mc.MovieId);

            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Category)
                .WithMany(c => c.MovieCategories)
                .HasForeignKey(mc => mc.CategoryId);

            modelBuilder.Entity<WatchlistMovie>()
                .HasKey(wm => new { wm.WatchlistId, wm.MovieId });

            modelBuilder.Entity<WatchlistMovie>()
                .HasOne(wm => wm.Watchlist)
                .WithMany(w => w.WatchlistMovies)
                .HasForeignKey(wm => wm.WatchlistId);

            modelBuilder.Entity<WatchlistMovie>()
                .HasOne(wm => wm.Movie)
                .WithMany(m => m.WatchlistMovies)
                .HasForeignKey(wm => wm.MovieId);
        }
    }
}