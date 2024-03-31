using DTOs;
using Model;
using System.Collections.Generic;

namespace BusinessLogic.ModelConversion {

    // This class is responsible for converting between Book DTOs and Book models.
    public class BookDtoConvert {

        // Converts a BookInDto object to a Book object.
        // Returns null if the input BookInDto is null.
        public static Book? FromDtoToBook(BookInDto bookDto) {
            Book? convertedBook = null;
            if (bookDto != null) {
                // Initialize a new Book object from the properties of the given BookInDto.
                convertedBook = new Book(
                    bookDto.Title,
                    bookDto.Author,
                    bookDto.Genre,
                    bookDto.NoOfPages,
                    bookDto.BookType,
                    bookDto.IsbnNo,
                    bookDto.Location,
                    bookDto.Status,
                    bookDto.ImageURL
                    );
            }
            return convertedBook;
        }

        // Converts a Book object to a BookOutDto object.
        // Returns null if the input Book is null.
        public static BookOutDto? FromBookToDto(Book bookModel) {
            BookOutDto? bookOut = null;
            if (bookModel != null) {
                // Initialize a new BookOutDto object from the properties of the given Book.
                bookOut = new BookOutDto(
                    bookModel.BookId,
                    bookModel.Title ?? string.Empty,
                    bookModel.Author ?? string.Empty,
                    bookModel.Genre,
                    bookModel.NoOfPages,
                    bookModel.BookType ?? string.Empty,
                    bookModel.IsbnNo ?? string.Empty,
                    bookModel.Location,
                    bookModel.Status ?? string.Empty,
                    bookModel.ImageURL
                );
            }
            return bookOut;
        }

        // Converts a list of Book objects to a list of BookOutDto objects.
        // Returns null if the input list is null.
        public static List<BookOutDto>? FromBookDtoToList(List<Book> bookModels) {
            List<BookOutDto>? aListOfDtos = null;
            if (bookModels != null) {
                aListOfDtos = new List<BookOutDto>();
                BookOutDto tempDto;
                // Loop through each Book object to convert it to a BookOutDto object.
                foreach (Book book in bookModels) {
                    if (book != null) {
                        tempDto = FromBookToDto(book);
                        // Add the converted BookOutDto to the list.
                        aListOfDtos.Add(tempDto);
                    }
                }
            }
            return aListOfDtos;
        }

    }
}
