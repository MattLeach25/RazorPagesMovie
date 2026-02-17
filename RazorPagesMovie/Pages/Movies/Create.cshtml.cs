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
        private readonly Meter? _meter;
        private readonly Counter<int>? _moviesCreatedCounter;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, ActivitySource? activitySource = null, Meter? meter = null)
        {
            _context = context;
            _activitySource = activitySource;
            _meter = meter;
            _moviesCreatedCounter = _meter?.CreateCounter<int>("MoviesCreated", description: "Number of movies created");
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
            
            // Track movie created event using Activity
            using (var activity = _activitySource?.StartActivity("MovieCreated"))
            {
                activity?.SetTag("Title", Movie.Title ?? "Untitled");
                activity?.SetTag("Genre", Movie.Genre ?? "Unknown");
            }

            if (Movie.isFavourite)
            {
                // Track favourite movie event using Activity
                using (var activity = _activitySource?.StartActivity("favouriteMovie"))
                {
                    activity?.SetTag("Title", Movie.Title ?? "Untitled");
                    activity?.SetTag("Genre", Movie.Genre ?? "Unknown");
                }
            }

            // Track metric
            _moviesCreatedCounter?.Add(1);

            return RedirectToPage("./Index");
        }
    }
}
