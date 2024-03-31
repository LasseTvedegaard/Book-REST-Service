using Model;

namespace BusinessLogic.Interfaces {
    public interface ILogControl {

        Task<int> Create(Log entity, string listType);
        Task<List<Log>> GetLogsByUserId(string userId);
        Task<List<Log>> GetAllLogs(string userId);
        Task<Log> GetLogById(int logId, string listType);
        Task<int> Update(int logId, Log updatedLog);
    }
}
