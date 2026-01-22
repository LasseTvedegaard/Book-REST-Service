using BusinessLogic.Interfaces;
using BusinessLogic.ModelConversion;
using DataAccess.Interfaces;
using DTOs;
using Model;

namespace BusinessLogic
{
    public class BookControl : IBookControl
    {
        private readonly IBookAccess _bookAccess;

        public BookControl(IBookAccess bookAccess)
        {
            _bookAccess = bookAccess;
        }

        // -----------------------------
        // CREATE
        // -----------------------------
        public async Task<int> Create(BookInDto entity, string userId)
        {
            if (entity == null || string.IsNullOrWhiteSpace(userId))
                return -1;

            Book? book = BookDtoConvert.FromDtoToBook(entity);
            if (book == null)
                return -1;

            // 🔒 Sæt ejer eksplicit her
            book.UserId = userId;

            return await _bookAccess.Create(book);
        }

        // -----------------------------
        // GET BY ID (USER-SCOPED)
        // -----------------------------
        public async Task<BookOutDto?> Get(int id, string userId)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(userId))
                return null;

            Book? book = await _bookAccess.Get(id, userId);
            if (book == null)
                return null;

            return BookDtoConvert.FromBookToDto(book);
        }

        // -----------------------------
        // GET ALL / FILTER BY STATUS (USER-SCOPED)
        // -----------------------------
        public async Task<List<BookOutDto>> GetAll(string userId, string? status = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<BookOutDto>();

            List<Book> books = !string.IsNullOrWhiteSpace(status)
                ? await _bookAccess.GetByStatus(status, userId)
                : await _bookAccess.GetAll(userId);

            return BookDtoConvert.FromBookDtoToList(books);
        }

        // -----------------------------
        // FULL UPDATE (USER-SCOPED)
        // -----------------------------
        public async Task<bool> Update(int id, BookInDto entity, string userId)
        {
            if (id <= 0 || entity == null || string.IsNullOrWhiteSpace(userId))
                return false;

            Book? bookToUpdate = BookDtoConvert.FromDtoToBook(entity);
            if (bookToUpdate == null)
                return false;

            // 🔒 Ejer må aldrig komme fra client – sæt den her
            bookToUpdate.UserId = userId;

            return await _bookAccess.Update(id, bookToUpdate, userId);
        }

        // -----------------------------
        // GET BY STATUS (USER-SCOPED)
        // -----------------------------
        public async Task<List<BookOutDto>> GetByStatus(string status, string userId)
        {
            if (string.IsNullOrWhiteSpace(status) || string.IsNullOrWhiteSpace(userId))
                return new List<BookOutDto>();

            List<Book> books = await _bookAccess.GetByStatus(status, userId);

            return BookDtoConvert.FromBookDtoToList(books);
        }

        // -----------------------------
        // STATUS UPDATE (USER-SCOPED)
        // -----------------------------
        public async Task<bool> UpdateStatus(int id, string status, string userId)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(status) || string.IsNullOrWhiteSpace(userId))
                return false;

            return await _bookAccess.UpdateStatus(id, status.Trim(), userId);
        }
    }
}
