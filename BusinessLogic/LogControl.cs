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

        // -----------------------------
        // CREATE
        // -----------------------------
        public async Task<int> Create(Log entity, string listType)
        {
            entity.ListType = listType;
            return await _logAccess.Create(entity);
        }

        // -----------------------------
        // GET BY ID
        // -----------------------------
        public async Task<Log> GetLogById(int logId, string listType)
        {
            return await _logAccess.GetLogById(logId, listType);
        }

        // -----------------------------
        // GET LOGS BY USER + LIST TYPE
        // -----------------------------
        public async Task<IEnumerable<Log>> GetLogsByUser(Guid userId, string listType)
        {
            return await _logAccess.GetLogsByUser(userId, listType);
        }

        // -----------------------------
        // GET LATEST LOGS
        // -----------------------------
        public async Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(Guid userId, string listType)
        {
            return await _logAccess.GetLatestLogsByUserAndListType(userId, listType);
        }

        // -----------------------------
        // GET ALL LOGS (HISTORY)
        // -----------------------------
        public async Task<List<Log>> GetAllLogs(Guid userId)
        {
            return await _logAccess.GetAllLogs(userId);
        }
    }
}
