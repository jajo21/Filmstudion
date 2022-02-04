using Filmstudion.Models.Interfaces;

namespace Filmstudion.Models.Film
{
    public class Film : IFilm
    {
        public int FilmId { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public string Country { get; set; }
        public int ReleaseYear { get; set; }
        public string FilmCopies { get; set; }
    }
}