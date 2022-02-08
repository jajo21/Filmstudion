using System.Collections.Generic;
using System.Text.Json.Serialization;
using Filmstudion.Models;

namespace Filmstudion.Resources
{
    public class FilmResponseResource : IFilm
    {
        public int FilmId { get; set; }
        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public string Country { get; set; }
        [JsonIgnore]
        public string Director { get; set; }
        [JsonIgnore]
        public IEnumerable<FilmCopy> FilmCopies { get; set; }
    }
}