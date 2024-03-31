using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic {
    public class LogControl : ILogControl {
        private readonly ILogAccess _logAccess;

        public LogControl(ILogAccess logAccess) {
            _logAccess = logAccess;
        }

        public async Task<int> Create(Log entity, string listType) {
            int insertedId = -1;
            insertedId = await _logAccess.Create(entity, listType);
            return insertedId;
        }

        public Task<List<Log>> GetAllLogs(string userId) {
            throw new NotImplementedException();
        }

        public async Task<Log> GetLogById(int logId, string listType) {
            Log foundLog;
            foundLog = await _logAccess.GetLogById(logId, listType);
            return foundLog;
        }

        public Task<List<Log>> GetLogsByUserId(string userId) {
            throw new NotImplementedException();
        }

        public async Task<int> Update(int logId, Log updatedLog) {
            return await _logAccess.Update(logId, updatedLog);
        }
    }
}
