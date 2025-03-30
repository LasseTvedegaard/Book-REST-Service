using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await _logAccess.Create(entity, listType);
        }

        public Task<List<Log>> GetAllLogs(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Log> GetLogById(int logId, string listType)
        {
            return await _logAccess.GetLogById(logId, listType);
        }

        public Task<List<Log>> GetLogsByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Update(int logId, Log updatedLog)
        {
            return await _logAccess.Update(logId, updatedLog);
        }

        // ✅ Her er den nye metode - korrekt placeret inde i klassen
        public async Task<IEnumerable<Log>> GetLogsByUser(Guid userId, string listType)
        {
            return await _logAccess.GetLogsByUser(userId, listType);
        }
        public async Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(Guid userId, string listType)
        {
            return await _logAccess.GetLatestLogsByUserAndListType(userId, listType);
        }


    }
}
