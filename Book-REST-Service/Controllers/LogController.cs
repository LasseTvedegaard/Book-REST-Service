using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using DTOs;

namespace Book_REST_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogControl _logControl;
        private readonly ILogger<LogController>? _logger;

        public LogController(ILogControl logControl, ILogger<LogController>? logger = null)
        {
            _logControl = logControl;
            _logger = logger;
        }

        // ✅ POST api/log
        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] LogCreateDto logDto)
        {
            if (logDto == null) return BadRequest();

            var logToCreate = new Log
            {
                BookId = logDto.BookId,
                UserId = logDto.UserId,
                CurrentPage = logDto.CurrentPage,
                NoOfPages = logDto.NoOfPages,
                ListType = logDto.ListType
            };

            int returnCode = await _logControl.Create(logToCreate, logDto.ListType);

            return returnCode switch
            {
                >= 0 => CreatedAtAction("Get", new { id = returnCode, listType = logDto.ListType }, logToCreate),
                -500 => StatusCode(500),
                _ => BadRequest()
            };
        }

        // ✅ GET api/log/5?listType=reading
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> Get(int id, [FromQuery] string listType)
        {
            var foundLog = await _logControl.GetLogById(id, listType);
            return foundLog != null ? Ok(foundLog) : NoContent();
        }


        // ✅ GET api/log/user/{userId}/all?listType=reading
        [HttpGet("user/{userId}/all")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogsByUser(Guid userId, [FromQuery] string listType)
        {
            var logs = await _logControl.GetLogsByUser(userId, listType);
            return (logs == null || !logs.Any()) ? NoContent() : Ok(logs);
        }

        // ✅ GET api/log/user/{userId}/latest?listType=reading
        [HttpGet("user/{userId}/latest")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLatestLogs(Guid userId, [FromQuery] string listType)
        {
            var logs = await _logControl.GetLatestLogsByUserAndListType(userId, listType);
            return (logs == null || !logs.Any()) ? NoContent() : Ok(logs);
        }
    }
}
