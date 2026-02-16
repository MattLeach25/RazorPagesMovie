using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Documentaries
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Documentary> Documentary { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? search { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool SearchFavourite { get; set; }

        public string NameSort { get; set; }

        public async Task OnGetAsync()
        {
            var documentaries = from d in _context.Documentary
                         select d;
            if (!string.IsNullOrEmpty(search))
            {
                documentaries = documentaries.Where(s => s.Title.Contains(search));
            }
            else if (SearchFavourite)
            {
                documentaries = documentaries.Where(s => s.isFavourite);
            }
                Documentary = await documentaries.ToListAsync();
        }
    }
}
