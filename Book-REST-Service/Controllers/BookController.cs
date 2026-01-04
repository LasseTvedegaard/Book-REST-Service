using BusinessLogic.Interfaces;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_REST_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookControl _bookControl;
        private readonly ILogger<BookController>? _logger;

        public BookController(IBookControl bookControl, ILogger<BookController>? logger = null)
        {
            _bookControl = bookControl;
            _logger = logger;
        }

        // GET: api/<BooksController>
        [HttpGet]
        [AllowAnonymous] // Offentlig adgang
        public async Task<ActionResult<List<BookOutDto>>> GetAll([FromQuery] string? status)  // Tilføjet query parameter til status
        {
            ActionResult<List<BookOutDto>> foundReturn;
            List<BookOutDto>? foundDtos = await _bookControl.GetAll(status);  // Videregiv status til BookControl

            if (foundDtos != null)
            {
                foundReturn = Ok(foundDtos);
            } else
            {
                foundDtos = new List<BookOutDto>();
                foundReturn = Ok(foundDtos.ToList());
            }

            return foundReturn;
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Offentlig adgang
        public async Task<ActionResult<BookOutDto>> Get(int id)
        {
            ActionResult<BookOutDto> foundReturn;

            BookOutDto? foundBookDto = await _bookControl.Get(id);

            if (foundBookDto != null)
            {
                foundReturn = Ok(foundBookDto);
            } else
            {
                foundReturn = new StatusCodeResult(204);
            }
            return foundReturn;
        }

        [HttpPost]
        [Authorize] // Kun for loggede brugere
        public async Task<ActionResult<BookOutDto>> CreateBook([FromBody] BookInDto bookToCreate)
        {
            if (bookToCreate == null)
            {
                _logger.LogWarning("Attempted to create a book with null data.");
                return BadRequest("Invalid book data.");
            }

            try
            {
                _logger.LogInformation("Creating a new book with title: {Title}", bookToCreate.Title);

                bookToCreate.Title = bookToCreate.Title?.Trim();
                bookToCreate.Author = bookToCreate.Author?.Trim();
                bookToCreate.BookType = bookToCreate.BookType?.Trim();
                bookToCreate.IsbnNo = bookToCreate.IsbnNo?.Trim();
                bookToCreate.Status = bookToCreate.Status?.Trim();
                bookToCreate.ImageURL = bookToCreate.ImageURL?.Trim();

                int insertedId = await _bookControl.Create(bookToCreate);

                if (insertedId > 0)
                {
                    _logger.LogInformation("Successfully inserted book with ID: {BookId}", insertedId);

                    BookOutDto? insertedBook = await _bookControl.Get(insertedId);

                    if (insertedBook != null)
                    {
                        _logger.LogInformation("Retrieved inserted book with ID: {BookId}", insertedId);
                        return CreatedAtAction(nameof(GetAll), new { id = insertedId }, insertedBook);
                    } else
                    {
                        _logger.LogError("Failed to retrieve the inserted book with ID: {BookId}", insertedId);
                        return StatusCode(500, "Failed to retrieve the inserted book.");
                    }
                } else
                {
                    _logger.LogError("Failed to insert the book with title: {Title}", bookToCreate.Title);
                    return StatusCode(500, "Failed to insert the book.");
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the book with title: {Title}", bookToCreate.Title);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // PUT api/<BooksController>/5
        [HttpPut("{id}")]
        [Authorize] // Kun for loggede brugere
        public async Task<ActionResult<BookOutDto>> UpdateBook(int id, [FromBody] BookInDto bookToUpdate)
        {
            if (bookToUpdate == null || id <= 0)
            {
                // Hvis ingen data modtages eller id er ugyldigt, returner BadRequest
                return BadRequest("Invalid book data or book ID.");
            }

            try
            {
                // Opdaterer bogen gennem BookControl
                bool isUpdated = await _bookControl.Update(id, bookToUpdate);

                if (isUpdated)
                {
                    // Find den opdaterede bog og returner den som BookOutDto
                    BookOutDto updatedBook = await _bookControl.Get(id);
                    return Ok(updatedBook);
                } else
                {
                    // Hvis opdatering ikke lykkes, returner en fejlstatus
                    return StatusCode(500, "Failed to update the book.");
                }
            } catch (Exception ex)
            {
                // Håndter fejl og log dem
                _logger?.LogError(ex, "An error occurred while updating the book with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPatch("status/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBookStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Status cannot be empty.");
            }

            var success = await _bookControl.UpdateStatus(id, status);

            if (success) return Ok();
            return StatusCode(500, "Failed to update book status.");
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateBookStatus(
    int id,
    [FromBody] UpdateBookStatusDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest("Status cannot be empty.");

            var success = await _bookControl.UpdateStatus(id, dto.Status.Trim());

            if (success)
                return NoContent(); // 204 – korrekt til update

            return StatusCode(500, "Failed to update book status.");
        }


    }
}
