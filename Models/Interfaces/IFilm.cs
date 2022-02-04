namespace Filmstudion.Models
{
    public interface IFilm
    {
        public int FilmId { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public string Country { get; set; }
        public int ReleaseYear { get; set; }
        public int FilmCopies { get; set; }
    }
}