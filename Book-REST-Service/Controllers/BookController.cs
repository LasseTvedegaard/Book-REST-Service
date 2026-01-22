using BusinessLogic.Interfaces;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Book_REST_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // 🔒 ALT i denne controller kræver login
    public class BookController : ControllerBase
    {
        private readonly IBookControl _bookControl;
        private readonly ILogger<BookController>? _logger;

        public BookController(IBookControl bookControl, ILogger<BookController>? logger = null)
        {
            _bookControl = bookControl;
            _logger = logger;
        }

        // -----------------------------
        // GET ALL (USER-SCOPED)
        // -----------------------------
        [HttpGet]
        public async Task<ActionResult<List<BookOutDto>>> GetAll([FromQuery] string? status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var books = await _bookControl.GetAll(userId, status);

            return Ok(books);
        }

        // -----------------------------
        // GET BY ID (USER-SCOPED)
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<BookOutDto>> Get(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var book = await _bookControl.Get(id, userId);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // -----------------------------
        // CREATE (USER-SCOPED)
        // -----------------------------
        [HttpPost]
        public async Task<ActionResult<BookOutDto>> CreateBook([FromBody] BookInDto bookToCreate)
        {
            if (bookToCreate == null)
                return BadRequest("Invalid book data.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                // Trim input
                bookToCreate.Title = bookToCreate.Title?.Trim();
                bookToCreate.Author = bookToCreate.Author?.Trim();
                bookToCreate.BookType = bookToCreate.BookType?.Trim();
                bookToCreate.IsbnNo = bookToCreate.IsbnNo?.Trim();
                bookToCreate.Status = bookToCreate.Status?.Trim();
                bookToCreate.ImageURL = bookToCreate.ImageURL?.Trim();

                var insertedId = await _bookControl.Create(bookToCreate, userId);

                if (insertedId <= 0)
                    return StatusCode(500, "Failed to insert the book.");

                var insertedBook = await _bookControl.Get(insertedId, userId);

                if (insertedBook == null)
                    return StatusCode(500, "Failed to retrieve the inserted book.");

                return CreatedAtAction(nameof(Get), new { id = insertedId }, insertedBook);
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating book");
                return StatusCode(500, "Internal server error.");
            }
        }

        // -----------------------------
        // FULL UPDATE (USER-SCOPED)
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<BookOutDto>> UpdateBook(int id, [FromBody] BookInDto bookToUpdate)
        {
            if (bookToUpdate == null || id <= 0)
                return BadRequest("Invalid book data or book ID.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                var isUpdated = await _bookControl.Update(id, bookToUpdate, userId);

                if (!isUpdated)
                    return NotFound();   // Enten findes bogen ikke, eller den tilhører ikke user

                var updatedBook = await _bookControl.Get(id, userId);

                if (updatedBook == null)
                    return NotFound();

                return Ok(updatedBook);
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating book with ID {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        // -----------------------------
        // UPDATE STATUS (USER-SCOPED)
        // -----------------------------
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookStatus(int id, [FromBody] UpdateBookStatusDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest("Status cannot be empty.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var success = await _bookControl.UpdateStatus(id, dto.Status.Trim(), userId);

            if (success)
                return NoContent(); // 204

            return NotFound(); // Enten findes bogen ikke, eller den tilhører ikke user
        }
    }
}
