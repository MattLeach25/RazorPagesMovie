using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using Microsoft.ApplicationInsights;


namespace RazorPagesMovie.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly TelemetryClient _telemetry;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, TelemetryClient? telemetry = null)
        {
            _context = context;
            _telemetry = telemetry;
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
            _telemetry?.TrackEvent("MovieCreated", new Dictionary<string, string>
            {
                { "Title", Movie.Title ?? "Untitled" },
                { "Genre", Movie.Genre ?? "Unknown" },
            });

            if (Movie.isFavourite)
            {
                _telemetry?.TrackEvent("favouriteMovie", new Dictionary<string, string>
                {
                    { "Title", Movie.Title ?? "Untitled" },
                    { "Genre", Movie.Genre ?? "Unknown" }
                });
            }

            _telemetry.TrackMetric("MoviesCreated", 1);

            return RedirectToPage("./Index");
        }
    }
}
