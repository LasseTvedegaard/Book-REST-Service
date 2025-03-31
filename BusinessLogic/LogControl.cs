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
            entity.ListType = listType; // 👈 tilføj listType til objektet her
            return await _logAccess.Create(entity); // 👈 kun ét argument nu
        }


        public async Task<Log> GetLogById(int logId, string listType)
        {
            return await _logAccess.GetLogById(logId, listType);
        }

        public Task<List<Log>> GetLogsByUserId(string userId)
        {
            throw new NotImplementedException();
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
        public async Task<List<Log>> GetAllLogs(string userId)
        {
            return await _logAccess.GetAllLogs(userId);
        }


    }
}
