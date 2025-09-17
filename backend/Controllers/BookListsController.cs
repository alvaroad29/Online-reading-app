using AutoMapper;
using backend.Models;
using backend.Models.Dtos;
using backend.Models.Dtos.BookQueries;
using backend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookListsController : ControllerBase
    {
        private readonly IBookListRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public BookListsController(
            IBookListRepository repository,
            IMapper mapper,
            UserManager<User> userManager
        )
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Obtiene todas las listas de libros con paginación, filtros y ordenamiento
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponse<BookListDto>>> GetAllBookLists(
            [FromQuery] BookListQueryParameters parameters)
        {
            try
            {
                // Validación de parámetros
                if (parameters.PageNumber < 1) parameters.PageNumber = 1;
                if (parameters.PageSize < 1 || parameters.PageSize > 100) parameters.PageSize = 10;

                var includePrivate = User.IsInRole("Admin");
                var pagedResponse = await _repository.GetAllAsync(parameters, includePrivate);

                var bookListDtos = _mapper.Map<List<BookListDto>>(pagedResponse.Data);

                var response = new PagedResponse<BookListDto>(
                    bookListDtos,
                    pagedResponse.PageNumber,
                    pagedResponse.PageSize,
                    pagedResponse.TotalRecords);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al obtener las listas de libros",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene una lista de libros por ID, 
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookListDto>> GetBookList(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                // Obtener la lista para verificar si el usuario es el dueño
                var bookList = await _repository.GetByIdAsync(id, true); // true para incluir privadas

                if (bookList == null)
                {
                    return NotFound(new { message = $"No se encontró la lista de libros con ID {id}" });
                }

                // Verificar si puede ver la lista privada
                var canViewPrivate = isAdmin || bookList.CreatorId == userId;

                if (!bookList.IsPublic && !canViewPrivate)
                {
                    return NotFound(new { message = $"No se encontró la lista de libros con ID {id}" });
                }

                var bookListDto = _mapper.Map<BookListDto>(bookList);
                return Ok(bookListDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor al obtener la lista de libros", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una lista de libros por ID incluyendo los libros que conteine, 
        /// </summary>

        [HttpGet("{id}/detailed")]
        [AllowAnonymous]
        public async Task<ActionResult<BookListDetailDto>> GetBookListDetailed(
             int id,
             [FromQuery] BookListBooksQueryParameters parameters)
        {
            try
            {
                // Validación de parámetros
                if (parameters.PageNumber < 1) parameters.PageNumber = 1;
                if (parameters.PageSize < 1 || parameters.PageSize > 100) parameters.PageSize = 20;

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                var bookList = await _repository.GetByIdAsync(id, true);
                if (bookList == null)
                    return NotFound(new { message = $"No se encontró la lista de libros con ID {id}" });

                var canViewPrivate = isAdmin || bookList.CreatorId == userId;
                if (!bookList.IsPublic && !canViewPrivate)
                    return NotFound(new { message = $"No se encontró la lista de libros con ID {id}" });

                var pagedBooks = await _repository.GetBooksByListIdAsync(id, parameters);

                var bookListDto = _mapper.Map<BookListDetailDto>(bookList);
                bookListDto.Books = pagedBooks;

                return Ok(bookListDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene las listas de libros de un usuario específico
        /// </summary>
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookListDto>>> GetBookListsByUser(string userId)
        {
            // sin paginacion por que solo puede haber un maximo de 20 listas
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { message = "El ID de usuario es requerido" });
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                var includePrivate = isAdmin || currentUserId == userId;

                var bookLists = await _repository.GetByUserIdAsync(userId, includePrivate);
                var bookListDtos = _mapper.Map<IEnumerable<BookListDto>>(bookLists);

                return Ok(bookListDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor al obtener las listas de libros del usuario", error = ex.Message });
            }
        }


        /// <summary>
        /// Crea una nueva lista de libros
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BookListDto>> CreateBookList([FromBody] CreateBookListDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Usuario no encontrado" });
                }

                const int MAX_LISTS_PER_USER = 20; // limite

                var canCreateMore = await _repository.CanUserCreateMoreListsAsync(userId, MAX_LISTS_PER_USER);
                if (!canCreateMore)
                {
                    return BadRequest(new
                    {
                        message = $"Has alcanzado el límite máximo de {MAX_LISTS_PER_USER} listas por usuario"
                    });
                }

                var bookList = _mapper.Map<BookList>(createDto);
                bookList.CreatorId = userId;
                bookList.CreationDate = DateTime.Now;
                bookList.UpdateDate = DateTime.Now;
                bookList.IsActive = true;

                var createdBookList = await _repository.CreateAsync(bookList);
                var bookListDto = _mapper.Map<BookListDto>(createdBookList);

                return CreatedAtAction(nameof(GetBookList), new { id = bookListDto.Id }, bookListDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor al crear la lista de libros", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una lista de libros existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookList(int id, [FromBody] UpdateBookListDto updateDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                }

                var isAdmin = User.IsInRole("Admin");

                var existingBookList = await _repository.GetByIdAsync(id, true);
                if (existingBookList == null)
                {
                    return NotFound(new { message = $"No se encontró la lista de libros con ID {id}" });
                }

                if (!isAdmin && existingBookList.CreatorId != userId)
                    return Forbid();

                _mapper.Map(updateDto, existingBookList);
                existingBookList.UpdateDate = DateTime.Now;

                await _repository.UpdateAsync(existingBookList);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor al actualizar la lista de libros", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una lista de libros (borrado lógico)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookList(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                }

                var existingBookList = await _repository.GetByIdAsync(id, true);
                if (existingBookList == null)
                {
                    return NotFound(new { message = $"No se encontró la lista de libros con ID {id}" });
                }

                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && existingBookList.CreatorId != userId)
                    return Forbid();

                var result = await _repository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró la lista de libros con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor al eliminar la lista de libros", error = ex.Message });
            }
        }

        /// <summary>
        /// Agrega un libro a una lista
        /// </summary>
        [HttpPost("{bookListId}/books/{bookId}")]
        public async Task<IActionResult> AddBookToList(int bookListId, int bookId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                }

                var isAdmin = User.IsInRole("Admin");

                // solo owner o admin pueden agregar libros
                if (!isAdmin && !await _repository.IsOwnerAsync(bookListId, userId))
                    return Forbid();

                const int MAX_BOOKS = 100;
                var bookList = await _repository.GetByIdAsync(bookListId, true);
                if (bookList != null && bookList.BookCount >= MAX_BOOKS)
                {
                    return BadRequest(new
                    {
                        message = $"La lista ha alcanzado el límite máximo de {MAX_BOOKS} libros"
                    });
                }

                var result = await _repository.AddBookToListAsync(bookListId, bookId);
                if (!result)
                {
                    return BadRequest(new { message = "El libro ya existe en la lista" });
                }

                return Ok(new { message = "Libro agregado a la lista exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor al agregar el libro a la lista", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un libro de una lista
        /// </summary>
        [HttpDelete("{bookListId}/books/{bookId}")]
        public async Task<IActionResult> RemoveBookFromList(int bookListId, int bookId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                }

                var isAdmin = User.IsInRole("Admin");

                // Solo owner o admin pueden remover libros
                if (!isAdmin && !await _repository.IsOwnerAsync(bookListId, userId))
                    return Forbid();

                var result = await _repository.RemoveBookFromListAsync(bookListId, bookId);
                if (!result)
                {
                    return NotFound(new { message = "El libro no existe en la lista" });
                }

                return Ok(new { message = "Libro eliminado de la lista exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor al eliminar el libro de la lista", error = ex.Message });
            }
        }
        
        /// <summary>
        /// Obtiene información de límites de listas para el usuario actual
        /// </summary>
        [HttpGet("limits")]
        public async Task<ActionResult> GetUserListLimits()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });

                const int MAX_LISTS_PER_USER = 20;
                var currentCount = await _repository.GetUserBookListCountAsync(userId);
                var canCreateMore = currentCount < MAX_LISTS_PER_USER;

                return Ok(new {
                    currentCount = currentCount,
                    maxLimit = MAX_LISTS_PER_USER,
                    remaining = MAX_LISTS_PER_USER - currentCount,
                    canCreateMore = canCreateMore
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Error al obtener información de límites",
                    error = ex.Message 
                });
            }
        }

    }
}