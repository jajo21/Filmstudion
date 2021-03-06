using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Filmstudion.Models;

namespace Filmstudion.Resources
{
    public class EditFilmResource : IFilm
    {
        [JsonIgnore]
        public int FilmId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ReleaseYear { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Director { get; set; }
        [JsonIgnore]
        public int NumberOfCopies { get; set; }
        [JsonIgnore]
        public List<FilmCopy> FilmCopies { get; set; }
    }
}