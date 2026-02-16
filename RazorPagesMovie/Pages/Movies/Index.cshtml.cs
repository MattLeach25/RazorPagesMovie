using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesMovie.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Genres { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? MovieGenre { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool SearchFavourite { get; set; }

        public string NameSort { get; set; }
        public string DateSort { get; set; }

        public async Task OnGetAsync(string sortOrder)
        {
            NameSort = sortOrder == "name_desc" ? "" : "name_desc";
            DateSort = sortOrder == "date_desc" ? "" : "date_desc";

            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            var movies = from m in _context.Movie
                         select m;

            switch (sortOrder)
            {
                case "name_desc":
                    movies = movies.OrderByDescending(s => s.Title);
                    break;
                case "date_desc":
                    movies = movies.OrderByDescending(s => s.ReleaseDate);
                    break;
                default:
                    movies = movies.OrderBy(s => s.Title);
                    break;
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                movies = movies.Where(s => s.Title.Contains(SearchString));
            }
            else if (!string.IsNullOrEmpty(MovieGenre))
            {
                movies = movies.Where(s => s.Genre.Contains(MovieGenre));
            }
            else if (SearchFavourite)
            {
                movies = movies.Where(s=> s.isFavourite);
            }

                Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
                Movie = await movies.ToListAsync();
        }
    }
}
