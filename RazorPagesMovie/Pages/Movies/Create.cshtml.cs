using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;


namespace RazorPagesMovie.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly ActivitySource? _activitySource;
        private readonly Counter<long> _moviesCreatedCounter;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, ActivitySource? activitySource = null, Meter? meter = null)
        {
            _context = context;
            _activitySource = activitySource;
            _moviesCreatedCounter = meter?.CreateCounter<long>("movies.created", unit: "movies", description: "Number of movies created")!;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Movie Movie { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Movie.Add(Movie);
            await _context.SaveChangesAsync();
            
            // Create activity for MovieCreated event
            using (var activity = _activitySource?.StartActivity("MovieCreated", ActivityKind.Internal))
            {
                activity?.SetTag("movie.title", Movie.Title ?? "Untitled");
                activity?.SetTag("movie.genre", Movie.Genre ?? "Unknown");
                activity?.SetTag("event.name", "MovieCreated");
            }

            if (Movie.isFavourite)
            {
                // Create activity for favourite movie event
                using (var activity = _activitySource?.StartActivity("FavouriteMovie", ActivityKind.Internal))
                {
                    activity?.SetTag("movie.title", Movie.Title ?? "Untitled");
                    activity?.SetTag("movie.genre", Movie.Genre ?? "Unknown");
                    activity?.SetTag("event.name", "FavouriteMovie");
                }
            }

            // Record metric for movies created
            _moviesCreatedCounter?.Add(1, 
                new KeyValuePair<string, object?>("movie.genre", Movie.Genre ?? "Unknown"));

            return RedirectToPage("./Index");
        }
    }
}
