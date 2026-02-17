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

namespace RazorPagesMovie.Pages.Documentaries
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly ActivitySource? _activitySource;
        private readonly Meter? _meter;
        private readonly Counter<int>? _documentariesCreatedCounter;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, ActivitySource? activitySource = null, Meter? meter = null)
        {
            _context = context;
            _activitySource = activitySource;
            _meter = meter;
            _documentariesCreatedCounter = _meter?.CreateCounter<int>("DocumentariesCreated", description: "Number of documentaries created");
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Documentary Documentary { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Documentary.Add(Documentary);
            await _context.SaveChangesAsync();

            // Track documentary created event using Activity
            using (var activity = _activitySource?.StartActivity("Documentaries Created"))
            {
                activity?.SetTag("Title", Documentary.Title ?? "Untitled");
                activity?.SetTag("Platform", Documentary.Platform ?? "Unspecified");
            }

            if (Documentary.isFavourite)
            {
                // Track favourite documentary event using Activity
                using (var activity = _activitySource?.StartActivity("FavouriteDocumentary"))
                {
                    activity?.SetTag("Title", Documentary.Title ?? "Untitled");
                    activity?.SetTag("Platform", Documentary.Platform ?? "Unspecified");
                }
             
            }

            // Track metric
            _documentariesCreatedCounter?.Add(1);

            return RedirectToPage("./Index");
        }
    }
}
