using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Filmstudion.Models;
using Filmstudion.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Filmstudion.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FilmsController : ControllerBase
    {
        private IFilmRepository _filmRepository;
        private IUserRepository _userRepository;
        private IFilmStudioRepository _filmStudioRepository;
        private IFilmCopyRepository _filmCopyRepository;
        private readonly IMapper _mapper;


        public FilmsController(
            IFilmRepository filmRepository,
            IUserRepository userRepository,
            IFilmCopyRepository filmCopyRepository,
            IMapper mapper,
            IFilmStudioRepository filmStudioRepository)
        {
            _filmRepository = filmRepository;
            _userRepository = userRepository;
            _filmCopyRepository = filmCopyRepository;
            _mapper = mapper;
            _filmStudioRepository = filmStudioRepository;

        }

        [HttpPut]
        public IActionResult AddFilm(CreateFilmResource model)
        {
            try
            {
                var username = User.Identity.Name;
                var user = _userRepository.GetUser(username);

                if(!user.IsAdmin) return StatusCode(StatusCodes.Status401Unauthorized, "User is not admin");
                
                var film = _filmRepository.AddFilm(model);

                return Ok(film);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetFilms()
        {
            try
            {
                var username = User.Identity.Name;
                var user = _userRepository.GetUserWithoutException(username);
                var films = _filmRepository.AllFilms;
                foreach(var film in films )
                {
                    film.FilmCopies = _filmCopyRepository.GetFilmCopies(film.FilmId).ToList();
                }

                if(user == null)
                {
                    return Ok(_mapper.Map<FilmResponseResource[]>(films));
                }

                if(User.Identity.IsAuthenticated)
                {
                    return Ok(films.ToArray());
                }

                return BadRequest("Error getting films");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public IActionResult GetFilm(int id) 
        {
            try
            {
                var username = User.Identity.Name;
                var user = _userRepository.GetUserWithoutException(username);
                var film = _filmRepository.GetFilmById(id);
                film.FilmCopies = _filmCopyRepository.GetFilmCopies(film.FilmId).ToList();

                if(user == null)
                {
                    return Ok(_mapper.Map<FilmResponseResource>(film));
                }

                if(User.Identity.IsAuthenticated)
                {
                    return Ok(film);
                }

                return BadRequest("Error getting film");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult EditFilm(int id, EditFilmResource model)
        {
            try
            {
                var username = User.Identity.Name;
                var user = _userRepository.GetUser(username);

                if(!user.IsAdmin) return Unauthorized("Only admins allowed");

                var newFilm = _filmRepository.EditFilmById(id, model);
                var result = _mapper.Map<EditFilmResponseResource>(newFilm);
                result.FilmCopies = _filmCopyRepository.GetFilmCopies(id).ToList();
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        } 
        [HttpPatch("{id:int}")]
        public IActionResult EditFilmCopies(int id, EditFilmCopiesResource model)
        {
            try
            {
                var username = User.Identity.Name;
                var user = _userRepository.GetUser(username);

                if(!user.IsAdmin) return Unauthorized("Only admins allowed");

                _filmCopyRepository.EditFilmCopies(id, model);
                var result = _filmRepository.GetFilmById(id);
                result.FilmCopies = _filmCopyRepository.GetFilmCopies(id).ToList();
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        } 

        [HttpPost("rent")]
        public IActionResult RentFilm(int id, int studioId)
        {
            try
            {
                var username = User.Identity.Name;
                var user = _userRepository.GetUser(username);

                if(user.Role != "Filmstudio") return Unauthorized("Only FilmStudios is allowed to rent");

                if(user.FilmStudioId != studioId) return Unauthorized("Filmstudio-ID does not match this users Key/Bearer-token");

                var filmToRent = _filmRepository.GetFilmById(id);

                if(filmToRent == null) return Conflict("No film with that ID found");

                if(!_filmCopyRepository.isFilmCopyAvailable(id)) return Conflict("No copy available");

                if(_filmCopyRepository.isFilmRentedByThisFilmStudio(id, studioId)) 
                    return this.StatusCode(StatusCodes.Status403Forbidden, "Film is already rented by this filmstudio");

                var copy =  _filmCopyRepository.GetAvailableFilmCopy(id);
                var studio = _filmStudioRepository.GetFilmStudioById(studioId);

                _filmStudioRepository.RentAFilm(studio, copy);

                return Ok(new { message = "Successful"}); // Skicka tillbaka annat svar?

            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
        [HttpPost("return")]
        public IActionResult ReturnFilm(int id, int studioId)
        {
            try
            {
                var username = User.Identity.Name;
                var user = _userRepository.GetUser(username);

                if(user.Role != "Filmstudio") return Unauthorized("Only FilmStudios is allowed to return");

                if(user.FilmStudioId != studioId) return Unauthorized("Filmstudio-ID does not match this users Key/Bearer-token");

                var filmToReturn = _filmRepository.GetFilmById(id);

                if(filmToReturn == null) return Conflict("No film with that ID found");

                var copy =  _filmCopyRepository.GetRentedFilmCopy(id, studioId);

                if(copy == null) return Conflict("No film-copy with that ID rented");

                var studio = _filmStudioRepository.GetFilmStudioById(studioId);

                _filmStudioRepository.ReturnAFilm(studio, copy);

                return Ok(new { message = "Successful"}); // Skicka tillbaka annat svar?

            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
    }
}