using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesMovie.Pages.Movies
{
    public class SearchModel : PageModel
    {
        public void OnGet()
        {
            Console.WriteLine("This is your movie search page");
        }
    }
}
