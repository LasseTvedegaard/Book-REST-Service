using DTOs;

namespace BusinessLogic.Interfaces
{
    public interface IBookControl
    {
        Task<int> Create(BookInDto entity);
        Task<BookOutDto?> Get(int id);
        Task<List<BookOutDto>?> GetAll(string? status = null);  // Tilføjet statusparameter
        Task<bool> Update(int id, BookInDto entity);
        Task<List<BookOutDto>?> GetByStatus(string status);  // Tilføjet GetByStatus-metode
        Task<bool> UpdateStatus(int id, string status);

    }
}
