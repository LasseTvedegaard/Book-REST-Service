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

            try
            {
                Guid userId = User.GetUserId();

                var log = new Log
                {
                    BookId = logDto.BookId,
                    UserId = userId,
                    CurrentPage = logDto.CurrentPage,
                    NoOfPages = logDto.NoOfPages,
                    ListType = logDto.ListType
                };

                int result = await _logControl.Create(log, logDto.ListType);

                return result > 0
                    ? CreatedAtAction(nameof(GetById), new { id = result, listType = logDto.ListType }, log)
                    : StatusCode(500);
            } catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // -----------------------------
        // GET LOG BY ID
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string listType)
        {
            var log = await _logControl.GetLogById(id, listType);
            return log == null ? NotFound() : Ok(log);
        }

        // -----------------------------
        // GET ALL LOGS FOR CURRENT USER
        // -----------------------------
        [HttpGet("me/all")]
        public async Task<IActionResult> GetMyLogs([FromQuery] string listType)
        {
            try
            {
                Guid userId = User.GetUserId();
                var logs = await _logControl.GetLogsByUser(userId, listType);
                return Ok(logs ?? new List<Log>());
            } catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // -----------------------------
        // GET LATEST LOGS FOR CURRENT USER
        // -----------------------------
        [HttpGet("me/latest")]
        public async Task<IActionResult> GetMyLatestLogs([FromQuery] string listType)
        {
            try
            {
                Guid userId = User.GetUserId();
                var logs = await _logControl.GetLatestLogsByUserAndListType(userId, listType);
                return Ok(logs ?? new List<Log>());
            } catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching latest logs");
                return StatusCode(500, "Failed to fetch latest logs");
            }
        }

        // -----------------------------
        // GET LOG HISTORY
        // -----------------------------
        [HttpGet("me/history")]
        public async Task<IActionResult> GetMyHistory()
        {
            try
            {
                Guid userId = User.GetUserId();
                var logs = await _logControl.GetAllLogs(userId);
                return Ok(logs ?? new List<Log>());
            } catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching log history");
                return StatusCode(500, "Failed to fetch log history");
            }
        }
    }
}
