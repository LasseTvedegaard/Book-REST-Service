using BusinessLogic.Interfaces;
using DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Book_REST_Service.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase {

        private readonly IBookControl _bookControl;
        private readonly ILogger<BookController>? _logger;

        public BookController(IBookControl bookControl, ILogger<BookController>? logger = null) {
            _bookControl = bookControl;
            _logger = logger;
        }

        // This is a method that handles HTTP GET requests to retrieve a list of books

        // GET: api/<booksController>

        // Specifies that the method should handle GET request
        [HttpGet]
        public async Task<ActionResult<List<BookOutDto>>> GetAll() {

            // foundReturn is a variable that holds the returned response
            ActionResult<List<BookOutDto>> foundReturn;

            // foundDtos is a list (nullable) that stores the retrieved books
            List<BookOutDto>? foundDtos = null;

            // Tries to retrieve the books (async) via the _bookControl class
            foundDtos = await _bookControl.GetAll();

            // Null check (checks if the books were found)
            if (foundDtos != null) {

            // If the books are found, a HTTP statuscode 200 (OK) response with list of books is set to the foundReturn variable
                foundReturn = Ok(foundDtos);
            } else {

            // If there are no books in the db, an empty list is returned with the HTTP 200 (OK) response
                foundDtos = new List<BookOutDto>();
                foundReturn = Ok(foundDtos.ToList());
            }

            // The response is returned with either a populated or empty list
            return foundReturn;
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookOutDto>> Get(int id) {
            ActionResult<BookOutDto> foundReturn;

            BookOutDto? foundBookDto = await _bookControl.Get(id);

            if (foundBookDto != null) {
                foundReturn = Ok(foundBookDto);
            } else {
                foundReturn = new StatusCodeResult(204);
            }
            return foundReturn;
        }

        // POST api/<BooksController>
        [HttpPost]
        public async Task<ActionResult<BookOutDto>> CreateBook([FromBody] BookInDto bookToCreate) {
            ActionResult<BookOutDto> foundResult;
            int insertedId = -1;
            BookOutDto? insertedBook = null;

            if (bookToCreate != null) {
                insertedId = await _bookControl.Create(bookToCreate);

                if (insertedId > 0) {
                    insertedBook = await _bookControl.Get(insertedId);

                    if (insertedBook != null) {
                        foundResult = CreatedAtAction(nameof(GetAll), new { id = insertedId }, insertedBook);
                    } else {
                        foundResult = new StatusCodeResult(500);
                    }
                } else {
                    foundResult = new StatusCodeResult(500);    
                }
            } else {
                foundResult = BadRequest();
            }
            return foundResult;
        }

        // PUT api/<BooksController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateBook(int id, [FromBody] BookInDto bookToUpdate) {
            ActionResult<bool> foundResult;

            bool isUpdated = false;
            if (bookToUpdate != null && id > 0) { 
                isUpdated = await _bookControl.Update(id, bookToUpdate);
                if (isUpdated) {
                    foundResult = Ok(true);
                } else {
                    foundResult = new StatusCodeResult(500);
                }
            } else {
                foundResult = new StatusCodeResult(500);
            }
            return foundResult;
        }
    }
}
