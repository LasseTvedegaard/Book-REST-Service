using DTOs;

namespace BusinessLogic.Interfaces
{
    public interface IBookControl
    {
        Task<int> Create(BookInDto entity, string userId);
        Task<BookOutDto?> Get(int id, string userId);
        Task<List<BookOutDto>> GetAll(string userId, string? status = null);
        Task<bool> Update(int id, BookInDto entity, string userId);
        Task<List<BookOutDto>> GetByStatus(string status, string userId);
        Task<bool> UpdateStatus(int id, string status, string userId);

    }
}
