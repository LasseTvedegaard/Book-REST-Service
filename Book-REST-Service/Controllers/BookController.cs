﻿using BusinessLogic.Interfaces;
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

        // GET: api/<booksController>
        [HttpGet]
        [AllowAnonymous] // Offentlig adgang
        public async Task<ActionResult<List<BookOutDto>>> GetAll()
        {
            ActionResult<List<BookOutDto>> foundReturn;
            List<BookOutDto>? foundDtos = await _bookControl.GetAll();

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
        public async Task<ActionResult<bool>> UpdateBook(int id, [FromBody] BookInDto bookToUpdate)
        {
            ActionResult<bool> foundResult;
            bool isUpdated = false;

            if (bookToUpdate != null && id > 0)
            {
                isUpdated = await _bookControl.Update(id, bookToUpdate);

                if (isUpdated)
                {
                    foundResult = Ok(true);
                } else
                {
                    foundResult = new StatusCodeResult(500);
                }
            } else
            {
                foundResult = new StatusCodeResult(500);
            }

            return foundResult;
        }
    }
}
