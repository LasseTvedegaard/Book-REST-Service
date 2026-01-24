using Model;

namespace BusinessLogic.Interfaces
{
    public interface ILogControl
    {
        Task<int> Create(Log entity, string listType);

        Task<Log?> GetLogById(int logId, string listType);

        Task<IEnumerable<Log>> GetLogsByUser(string userId, string listType);

        Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(string userId, string listType);

        Task<List<Log>> GetAllLogs(string userId);

        Task<bool> Update(Log log, string listType);
    }
}
