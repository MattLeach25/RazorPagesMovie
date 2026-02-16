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

namespace RazorPagesMovie.Pages.Documentaries
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

            _telemetry?.TrackEvent("Documentaries Created", new Dictionary<string, string>
            {
                { "Title", Documentary.Title ?? "Untitled" },
                { "Platform", Documentary.Platform ?? "Unspecified" }
            });

            if (Documentary.isFavourite)
            {
                _telemetry?.TrackEvent("FavouriteDocumentary", new Dictionary<string, string>
                {
                    {"Title", Documentary.Title?? "Untitled" },
                    {"Platform", Documentary.Platform?? "Unspecified" }
                });
             
            }

            _telemetry.TrackMetric("DocumentariesCreated", 1);

            return RedirectToPage("./Index");
        }
    }
}
