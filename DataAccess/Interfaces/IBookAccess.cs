using Model;

namespace DataAccess.Interfaces {
    public interface IBookAccess {

        Task<int> Create(Book entity);
        Task<Book>? Get(int id, string userId);
        Task<List<Book>> GetAll(string userId);
        Task<bool> Update(int id, Book entity, string userId);
        Task<List<Book>> GetByStatus(string status, string userId);
        Task<bool> UpdateStatus(int id, string status, string userId);

    }
}
