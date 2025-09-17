using AutoMapper;
using backend.Models.Dtos;
using backend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace backend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]

    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreController(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _genreRepository.GetGenres();
            var genresDto = _mapper.Map<List<GenreDto>>(genres);
            return Ok(genresDto);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetGenre")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenre(int id)
        {
            var genre = await _genreRepository.GetGenre(id);

            if (genre == null)
            {
                return NotFound($"El genero con el id {id} no existe ");
            }
            var genreDto = _mapper.Map<GenreDto>(genre);
            return Ok(genreDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreDto createGenreDto)
        {
            if (createGenreDto == null)
            {
                return BadRequest(ModelState);
            }
            if (await _genreRepository.GenreExists(createGenreDto.Name))
            {
                ModelState.AddModelError("Custom Error", "El genero ya existe");
                return BadRequest(ModelState);
            }
            var genre = _mapper.Map<Genre>(createGenreDto);

            var created = await _genreRepository.CreateGenre(genre);
            if (!created)
            {
                ModelState.AddModelError("Custom Error", $"Algo salio mal al guardar el registro {genre.Name}");
                return StatusCode(500, ModelState);
            }

            var genreResponse = _mapper.Map<GenreDto>(genre);
            return CreatedAtRoute("GetGenre", new { id = genreResponse.Id }, genreResponse);
        }

        [HttpPatch("{id:int}", Name = "UpdateGenre")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] CreateGenreDto updateGenreDto)
        {
            // Validar DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _genreRepository.GenreExists(id))
            {
                return NotFound($"El genero con el id {id} no existe");
            }

            if (await _genreRepository.GenreExists(updateGenreDto.Name))
            {
                ModelState.AddModelError("Custom Error", "El genero ya existe");
                return BadRequest(ModelState);
            }

            var genre = _mapper.Map<Genre>(updateGenreDto);
            genre.Id = id;
            if (!await _genreRepository.UpdateGenre(genre))
            {
                ModelState.AddModelError("Custom Error", $"Algo salio mal al actualizar el registro {genre.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteGenre")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreRepository.GetGenre(id);
            if (genre == null)
            {
                return NotFound($"El genero con el id {id} no existe");
            }

            if (!await _genreRepository.DeleteGenre(genre))
            {
                ModelState.AddModelError("Custom Error", $"Algo salio mal al borrar el registro {genre.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}

