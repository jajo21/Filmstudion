using AutoMapper;
using Filmstudion.Models;
using Filmstudion.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Filmstudion.Controllers
{
    [ApiController]
    [Route("api/filmstudio")] // Ändra till [controller]??? Men i kraven står det filmstudio
    public class FilmStudiosController : ControllerBase
    {
        private IRegisterFilmStudioRepository _repository;
        private readonly IMapper _mapper;

        public FilmStudiosController(IRegisterFilmStudioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterFilmStudio model)
        {
            _repository.Register(model);
            return Ok(_mapper.Map<FilmStudio>(model));
        }
    }
}