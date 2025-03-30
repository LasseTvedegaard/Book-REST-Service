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

        public async Task<int> Create(BookInDto entity)
        {
            int createdId = -1;

            if (entity != null)
            {
                Book? internBook = BookDtoConvert.FromDtoToBook(entity);
                if (internBook != null)
                {
                    createdId = await _bookAccess.Create(internBook);
                }
            }
            return createdId;
        }

        public async Task<BookOutDto?> Get(int id)
        {
            BookOutDto? foundDto = null;

            if (id > 0)
            {
                Book? foundBook = await _bookAccess.Get(id);
                if (foundBook != null)
                {
                    foundDto = BookDtoConvert.FromBookToDto(foundBook);
                }
            }
            return foundDto;
        }

        public async Task<List<BookOutDto>?> GetAll(string? status = null)
        {
            List<BookOutDto>? foundDtos = null;
            List<Book> foundBooks;

            if (!string.IsNullOrEmpty(status))
            {
                foundBooks = await _bookAccess.GetByStatus(status);  // Fetch books filtered by status
            } else
            {
                foundBooks = await _bookAccess.GetAll();  // Fetch all books
            }

            if (foundBooks != null)
            {
                foundDtos = BookDtoConvert.FromBookDtoToList(foundBooks);
            }

            return foundDtos;
        }

        public async Task<bool> Update(int id, BookInDto entity)
        {
            bool isUpdated = false;

            if (entity != null)
            {
                Book? bookToUpdate = BookDtoConvert.FromDtoToBook(entity);
                if (bookToUpdate != null)
                {
                    isUpdated = await _bookAccess.Update(id, bookToUpdate);
                }
            }
            return isUpdated;
        }

        public async Task<List<BookOutDto>?> GetByStatus(string status)
        {
            List<BookOutDto>? foundDtos = null;

            List<Book> foundBooks = await _bookAccess.GetByStatus(status);

            if (foundBooks != null)
            {
                foundDtos = BookDtoConvert.FromBookDtoToList(foundBooks);
            }

            return foundDtos;
        }

        public async Task<bool> UpdateStatus(int id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            // Hent den eksisterende bog
            var existingBook = await _bookAccess.Get(id);
            if (existingBook == null)
                return false;

            // Opdater status
            existingBook.Status = status;

            // Gem opdateringen
            return await _bookAccess.Update(id, existingBook);
        }

    }
}
