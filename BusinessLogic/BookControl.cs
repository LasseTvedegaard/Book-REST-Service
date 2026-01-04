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
        public async Task<int> Create(BookInDto entity)
        {
            if (entity == null)
                return -1;

            Book? book = BookDtoConvert.FromDtoToBook(entity);
            if (book == null)
                return -1;

            return await _bookAccess.Create(book);
        }

        // -----------------------------
        // GET BY ID
        // -----------------------------
        public async Task<BookOutDto?> Get(int id)
        {
            if (id <= 0)
                return null;

            Book? book = await _bookAccess.Get(id);
            if (book == null)
                return null;

            return BookDtoConvert.FromBookToDto(book);
        }

        // -----------------------------
        // GET ALL / FILTER BY STATUS
        // -----------------------------
        public async Task<List<BookOutDto>?> GetAll(string? status = null)
        {
            List<Book> books = !string.IsNullOrWhiteSpace(status)
                ? await _bookAccess.GetByStatus(status)
                : await _bookAccess.GetAll();

            if (books == null)
                return null;

            return BookDtoConvert.FromBookDtoToList(books);
        }

        // -----------------------------
        // FULL UPDATE (EDIT BOOK)
        // -----------------------------
        public async Task<bool> Update(int id, BookInDto entity)
        {
            if (id <= 0 || entity == null)
                return false;

            Book? bookToUpdate = BookDtoConvert.FromDtoToBook(entity);
            if (bookToUpdate == null)
                return false;

            return await _bookAccess.Update(id, bookToUpdate);
        }

        // -----------------------------
        // GET BY STATUS (optional helper)
        // -----------------------------
        public async Task<List<BookOutDto>?> GetByStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            List<Book> books = await _bookAccess.GetByStatus(status);
            if (books == null)
                return null;

            return BookDtoConvert.FromBookDtoToList(books);
        }

        // -----------------------------
        // STATUS UPDATE (ROBUST)
        // -----------------------------
        public async Task<bool> UpdateStatus(int id, string status)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(status))
                return false;

            // Brug specialiseret DAL-metode
            return await _bookAccess.UpdateStatus(id, status.Trim());
        }
    }
}
