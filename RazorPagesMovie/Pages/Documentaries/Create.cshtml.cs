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
        private readonly Meter _meter;
        private readonly Counter<int> _documentariesCreatedCounter;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, ActivitySource activitySource, Meter meter)
        {
            _context = context;
            _activitySource = activitySource;
            _meter = meter;
            _documentariesCreatedCounter = _meter.CreateCounter<int>("documentaries_created", description: "Number of documentaries created");
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

            using var activity = _activitySource.StartActivity("CreateDocumentary");
            activity?.SetTag("documentary.title", Documentary.Title ?? "Untitled");
            activity?.SetTag("documentary.platform", Documentary.Platform ?? "Unspecified");

            _context.Documentary.Add(Documentary);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("Documentaries Created"));

            if (Documentary.isFavourite)
            {
                activity?.AddEvent(new ActivityEvent("FavouriteDocumentary"));
            }

            _documentariesCreatedCounter.Add(1);

            return RedirectToPage("./Index");
        }
    }
}
