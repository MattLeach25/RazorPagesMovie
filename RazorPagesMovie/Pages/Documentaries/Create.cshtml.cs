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
        private readonly ActivitySource _activitySource;
        private readonly Counter<long> _documentariesCreatedCounter;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, ActivitySource? activitySource = null, Meter? meter = null)
        {
            _context = context;
            _activitySource = activitySource ?? new ActivitySource("RazorPagesMovie");
            _documentariesCreatedCounter = meter?.CreateCounter<long>("documentaries.created", unit: "documentaries", description: "Number of documentaries created") 
                ?? new Meter("RazorPagesMovie").CreateCounter<long>("documentaries.created");
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

            // Create activity for DocumentariesCreated event
            using (var activity = _activitySource.StartActivity("DocumentariesCreated", ActivityKind.Internal))
            {
                activity?.SetTag("documentary.title", Documentary.Title ?? "Untitled");
                activity?.SetTag("documentary.platform", Documentary.Platform ?? "Unspecified");
                activity?.SetTag("event.name", "Documentaries Created");
            }

            if (Documentary.isFavourite)
            {
                // Create activity for favourite documentary event
                using (var activity = _activitySource.StartActivity("FavouriteDocumentary", ActivityKind.Internal))
                {
                    activity?.SetTag("documentary.title", Documentary.Title ?? "Untitled");
                    activity?.SetTag("documentary.platform", Documentary.Platform ?? "Unspecified");
                    activity?.SetTag("event.name", "FavouriteDocumentary");
                }
             
            }

            // Record metric for documentaries created
            _documentariesCreatedCounter.Add(1, 
                new KeyValuePair<string, object?>("documentary.platform", Documentary.Platform ?? "Unspecified"));

            return RedirectToPage("./Index");
        }
    }
}
