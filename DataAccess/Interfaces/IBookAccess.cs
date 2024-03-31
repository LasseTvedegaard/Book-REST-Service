using Model;

namespace DataAccess.Interfaces {
    public interface IBookAccess {

        Task<int> Create(Book entity);
        Task<Book>? Get(int id);
        Task<List<Book>> GetAll();
        Task<bool> Update(int id, Book entity);
    }
}
