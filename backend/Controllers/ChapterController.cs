using System.Security.Claims;
using AutoMapper;
using backend.Models;
using backend.Models.Dtos;
using backend.Models.Dtos.BookQueries;
using backend.Repository;
using backend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;
using System.IO;

namespace backend.Controllers
{
    // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]

    public class ChapterController : ControllerBase
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ChapterController(
            IChapterRepository chapterRepository,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _chapterRepository = chapterRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChapterDto>> GetChapter(int id)
        {
            // 1. Obtener el capítulo
            var chapter = await _chapterRepository.GetChapterByIdAsync(id);
            if (chapter == null)
            {
                return NotFound($"Capítulo con ID {id} no encontrado.");
            }

            // 2. Incrementar ViewCount (siempre)
            await _chapterRepository.IncrementViewCountAsync(chapter);

            // 3. Si el usuario está logueado, registrar en ReadingHistory
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                // Evitar duplicados: verificar si ya vio este capítulo
                var hasViewed = await _chapterRepository.HasUserViewedChapterAsync(id, currentUser.Id);
                if (!hasViewed)
                {
                    var history = new ReadingHistory
                    {
                        ChapterId = id,
                        UserId = currentUser.Id,
                        ViewDate = DateTime.UtcNow
                    };
                    await _chapterRepository.AddToReadingHistoryAsync(history);
                }
            }

            // 4. Calcular IDs de anterior y siguiente
            var previous = await _chapterRepository.GetPreviousChapterAsync(chapter.VolumeId, chapter.Order);
            var next = await _chapterRepository.GetNextChapterAsync(chapter.VolumeId, chapter.Order);

            // 5. Mapear a DTO y devolver
            var dto = _mapper.Map<ChapterReadDto>(chapter);
            dto.PreviousChapterId = previous?.Id;
            dto.NextChapterId = next?.Id;
            return Ok(dto);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadChapter([FromForm] CreateChapterFromFileDto dto)
        {
            // 1. Validar archivo presente
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");

            // 2. Validar tamaño máximo (ejemplo: 5 MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (dto.File.Length > maxFileSize)
                return BadRequest("File is too large. Maximum allowed size is 5 MB.");

            // 3. Validar tipo de archivo permitido
            var extension = Path.GetExtension(dto.File.FileName).ToLower();
            var allowedExtensions = new[] { ".txt", ".pdf", ".docx" };
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Unsupported file type. Only .txt, .pdf, .docx are allowed.");

            // 4. Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required.");
            if (dto.VolumeId <= 0)
                return BadRequest("VolumeId is required and must be greater than 0.");
            if (dto.Order < 0)
                return BadRequest("Order must be 0 or greater.");

            string content = "";
            try
            {
                using (var stream = dto.File.OpenReadStream())
                {
                    if (extension == ".txt")
                    {
                        using var reader = new StreamReader(stream);
                        content = await reader.ReadToEndAsync();
                    }
                    else if (extension == ".pdf")
                    {
                        using var pdf = PdfDocument.Open(stream);
                        foreach (var page in pdf.GetPages())
                            content += page.Text;
                    }
                    else if (extension == ".docx")
                    {
                        using var mem = new MemoryStream();
                        await stream.CopyToAsync(mem);
                        mem.Position = 0;
                        using var wordDoc = WordprocessingDocument.Open(mem, false);
                        content = wordDoc.MainDocumentPart?.Document?.Body?.InnerText ?? string.Empty;
                    }
                }
            }
            catch
            {
                // 5. Manejar errores de extracción de texto
                return BadRequest("Error extracting text from file. The file may be corrupt or unsupported.");
            }

            // 6. Validar que el contenido no esté vacío
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("The file does not contain readable text.");

            // 7. (Opcional) Validar que no exista un capítulo igual en el mismo volumen
            // var exists = await _chapterRepository.ExistsAsync(dto.Title, dto.VolumeId, dto.Order);
            // if (exists)
            //     return BadRequest("A chapter with the same title and order already exists in this volume.");

            // Calcular automáticamente el siguiente Order disponible
            int nextOrder = await _chapterRepository.GetNextOrderForVolumeAsync(dto.VolumeId);

            var chapter = new Chapter
            {
                Title = dto.Title,
                Content = content,
                VolumeId = dto.VolumeId,
                Order = nextOrder,
                PublishDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                ViewCount = 0,
                IsActive = true
            };

            var createdChapter = await _chapterRepository.CreateChapterAsync(chapter);

            return Ok(createdChapter);
        }

    }
}
