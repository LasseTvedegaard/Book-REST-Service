using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;

namespace BusinessLogic
{
    public class LogControl : ILogControl
    {
        private readonly ILogAccess _logAccess;

        public LogControl(ILogAccess logAccess)
        {
            _logAccess = logAccess;
        }

        public async Task<int> Create(Log entity, string listType)
        {
            entity.ListType = listType;
            return await _logAccess.Create(entity);
        }

        public async Task<Log?> GetLogById(int logId, string listType)
        {
            return await _logAccess.GetLogById(logId, listType);
        }

        public async Task<IEnumerable<Log>> GetLogsByUser(string userId, string listType)
        {
            return await _logAccess.GetLogsByUser(userId, listType);
        }

        public async Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(string userId, string listType)
        {
            return await _logAccess.GetLatestLogsByUserAndListType(userId, listType);
        }

        public async Task<List<Log>> GetAllLogs(string userId)
        {
            return await _logAccess.GetAllLogs(userId);
        }

        public Task<bool> Update(Log log, string listType)
        {
            return _logAccess.Update(log, listType);
        }

    }
}
