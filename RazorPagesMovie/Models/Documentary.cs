using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Documentary
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]

        public string Genre { get; set; }

        public string Platform { get; set; }

        [Display(Name = "Favourite")]
        public bool isFavourite { get; set; }

    }
}
