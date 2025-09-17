using System.Security.Claims;
using AutoMapper;
using backend.Models;
using backend.Models.Dtos;
using backend.Models.Dtos.BookQueries;
using backend.Repository;
using backend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]

    public class VolumeController : ControllerBase
    {
        private readonly IVolumeRepository _volumeRepository;
        private readonly IMapper _mapper;


        public VolumeController(IVolumeRepository volumeRepository, IMapper mapper)
        {
            _volumeRepository = volumeRepository;
            _mapper = mapper;

        }

        // GET: api/books/1/volumes
        [HttpGet]
        public async Task<IActionResult> GetVolumesByBook(int bookId)
        {
            var volumes = await _volumeRepository.GetVolumesByBook(bookId);
            return Ok(volumes);
        }

        // POST: api/books/1/volumes
        [HttpPost]
        public async Task<IActionResult> CreateVolume(int bookId, [FromBody] VolumeCreateDto volumeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            volumeDto.BookId = bookId; // Asegurar consistencia
            var volume = await _volumeRepository.CreateVolume(volumeDto);
            
            return CreatedAtAction(nameof(GetVolumesByBook), new { bookId }, volume);
        }

        // PUT: api/books/1/volumes/5
        [HttpPut("{volumeId}")]
        public async Task<IActionResult> UpdateVolume(int bookId, int volumeId, [FromBody] VolumeUpdateDto volumeDto)
        {
            try
            {
                await _volumeRepository.UpdateVolume(volumeId, volumeDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/books/1/volumes/5
        [HttpDelete("{volumeId}")]
        public async Task<IActionResult> DeleteVolume(int bookId, int volumeId)
        {
            var result = await _volumeRepository.DeleteVolume(volumeId);
            return result ? NoContent() : NotFound();
        }



    }

}
