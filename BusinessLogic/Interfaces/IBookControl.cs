using DTOs;

namespace BusinessLogic.Interfaces {
    public interface IBookControl {

        Task<int> Create(BookInDto entity);
        Task<BookOutDto?> Get(int id);  
        Task<List<BookOutDto>?> GetAll();
        Task<bool> Update(int id, BookInDto entity);
    }
}
