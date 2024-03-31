using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase {

        private readonly ILogControl _logControl;
        private readonly ILogger<LogController>? _logger;

        public LogController(ILogControl logControl, ILogger<LogController>? logger = null) {
            _logControl = logControl;
            _logger = logger;
        }

        // POST api/<LogController>
        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] Log logToCreate) {
            IActionResult foundResult;
            
            int returnCode = -1;

            string listType = logToCreate.ListType;
            
            returnCode = await _logControl.Create(logToCreate, listType);
            
            switch (returnCode) {
                case >= 0:
                    logToCreate.LogId = returnCode;
                    foundResult = CreatedAtAction("Get", new { id = returnCode, listType }, logToCreate); // 201

                    break;
                case -500:
                    foundResult = StatusCode(500);
                    break;
                default:
                    foundResult = StatusCode(400);
                    break;
            }
            return foundResult;
        }

        // GET: api/<LogController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> Get(int id, string listType) {
            ActionResult<Log> foundReturn;
            Log foundLog = await _logControl.GetLogById(id, listType);
            if (foundLog != null) {
                foundReturn = Ok(foundLog);
            } else {
                foundReturn = StatusCode(204);
            }
            return foundReturn;
        }

        // PUT api/<LogController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLog(int id, [FromBody] Log updatedLog) {
            int returnCode = await _logControl.Update(id, updatedLog);
            IActionResult foundResult;

            switch (returnCode) {
                case 0:
                    foundResult = Ok();
                    break;
                case -1:
                    foundResult = NotFound();
                    break;
                case -500:
                    foundResult = StatusCode(500);
                    break;
                    default:
                    foundResult = StatusCode(400);
                    break;
            }
            return foundResult;
        }

    }
}
