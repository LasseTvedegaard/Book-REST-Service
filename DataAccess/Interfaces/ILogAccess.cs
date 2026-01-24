using Model;

namespace DataAccess.Interfaces
{
    public interface ILogAccess
    {
        Task<int> Create(Log entity);

        Task<Log?> GetLogById(int logId, string listType);

        Task<IEnumerable<Log>> GetLogsByUser(string userId, string listType);

        Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(string userId, string listType);

        Task<List<Log>> GetAllLogs(string userId);

        Task<bool> Update(Log log, string listType);
    }
}
