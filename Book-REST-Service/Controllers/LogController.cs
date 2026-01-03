using BusinessLogic.Interfaces;
using Book_REST_Service.Helpers;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly ILogControl _logControl;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogControl logControl, ILogger<LogController> logger)
        {
            _logControl = logControl;
            _logger = logger;
        }

        // -----------------------------
        // CREATE LOG (JWT USER)
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] LogCreateDto logDto)
        {
            if (logDto == null)
                return BadRequest();

            var userId = User.GetUserId();

            var log = new Log
            {
                BookId = logDto.BookId,
                UserId = userId,
                CurrentPage = logDto.CurrentPage,
                NoOfPages = logDto.NoOfPages,
                ListType = logDto.ListType
            };

            int result = await _logControl.Create(log, logDto.ListType);

            return result switch
            {
                > 0 => CreatedAtAction(nameof(GetById), new { id = result, listType = logDto.ListType }, log),
                -500 => StatusCode(500),
                _ => BadRequest()
            };
        }

        // -----------------------------
        // GET LOG BY ID
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> GetById(int id, [FromQuery] string listType)
        {
            var log = await _logControl.GetLogById(id, listType);
            return log == null ? NoContent() : Ok(log);
        }

        // -----------------------------
        // GET ALL LOGS FOR CURRENT USER
        // -----------------------------
        [HttpGet("me/all")]
        public async Task<IActionResult> GetMyLogs([FromQuery] string listType)
        {
            var userId = User.GetUserId();

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest("Invalid user ID format.");
            }

            var logs = await _logControl.GetLogsByUser(userGuid, listType);

            return logs == null || !logs.Any()
                ? NoContent()
                : Ok(logs);
        }

        // -----------------------------
        // GET LATEST LOGS FOR CURRENT USER
        // -----------------------------
        [HttpGet("me/latest")]
        public async Task<IActionResult> GetMyLatestLogs([FromQuery] string listType)
        {
            var userId = User.GetUserId();

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest("Invalid user ID format.");
            }

            var logs = await _logControl.GetLatestLogsByUserAndListType(userGuid, listType);

            return logs == null || !logs.Any()
                ? NoContent()
                : Ok(logs);
        }

        // -----------------------------
        // GET LOG HISTORY (CURRENT USER)
        // -----------------------------
        [HttpGet("me/history")]
        public async Task<IActionResult> GetMyHistory()
        {
            try
            {
                var userId = User.GetUserId();
                var logs = await _logControl.GetAllLogs(userId);
                return Ok(logs);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching log history");
                return StatusCode(500, "Failed to fetch log history");
            }
        }
    }
}
