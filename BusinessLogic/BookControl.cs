using BusinessLogic.Interfaces;
using BusinessLogic.ModelConversion;
using DataAccess.Interfaces;
using DTOs;
using Model;

namespace BusinessLogic {
    public class BookControl : IBookControl {
        private readonly IBookAccess _bookAccess;

        public BookControl(IBookAccess bookAccess) {
            _bookAccess = bookAccess;
        }
        public async Task<int> Create(BookInDto entity) {
            int createdId = -1;

            if (entity != null) {
                Book? internBook = BookDtoConvert.FromDtoToBook(entity);
                if (internBook != null) {
                    createdId = await _bookAccess.Create(internBook);
                }
            }
            return createdId;
        }

        public async Task<BookOutDto?> Get(int id) {
            BookOutDto? foundDto = null;

            if (id > 0) {
                Book? foundBook = await _bookAccess.Get(id);
                if (foundBook != null) {
                    foundDto = BookDtoConvert.FromBookToDto(foundBook);
                }
            }
            return foundDto;
        }

        // This method is in the businessLogic layer and tries to retrieve a list of all books from the database
        public async Task<List<BookOutDto>?> GetAll() {

            // Initialize a nullable list to store the retrieved BookOutDto objects.
            List<BookOutDto>? foundDtos = null;

            // Retrieve books asynchronously using the _bookAccess class.
            List<Book> foundBooks = await _bookAccess.GetAll();

            // Check if any books were retrieved.
            if (foundBooks != null) {

                // Convert the list of Book objects to a list of BookOutDto objects.
                // This filters the information that will be sent to the frontend.
                foundDtos = BookDtoConvert.FromBookDtoToList(foundBooks);
            }
            return foundDtos;
        }

        public async Task<bool> Update(int id, BookInDto entity) {
            bool isUpdated = false;

            if (entity != null) {
                Book? bookToUpdate = BookDtoConvert.FromDtoToBook(entity);
                if (bookToUpdate != null) {
                    isUpdated = await _bookAccess.Update(id, bookToUpdate);
                }
            }
            return isUpdated;
        }
    }
}
