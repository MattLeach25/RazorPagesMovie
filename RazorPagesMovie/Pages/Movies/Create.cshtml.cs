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
        private readonly ActivitySource _activitySource;
        private readonly Meter _meter;
        private readonly Counter<int> _moviesCreatedCounter;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, ActivitySource activitySource, Meter meter)
        {
            _context = context;
            _activitySource = activitySource;
            _meter = meter;
            _moviesCreatedCounter = _meter.CreateCounter<int>("movies_created", description: "Number of movies created");
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

            using var activity = _activitySource.StartActivity("CreateMovie");
            activity?.SetTag("movie.title", Movie.Title ?? "Untitled");
            activity?.SetTag("movie.genre", Movie.Genre ?? "Unknown");

            _context.Movie.Add(Movie);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("MovieCreated"));

            if (Movie.isFavourite)
            {
                activity?.AddEvent(new ActivityEvent("favouriteMovie"));
            }

            _moviesCreatedCounter.Add(1);

            return RedirectToPage("./Index");
        }
    }
}
