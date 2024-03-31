using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase {
        private readonly ICRUD<Employee> _employeeControl;
        private readonly ILogger<EmployeeController>? _logger;

        public EmployeeController(ICRUD<Employee> employeeControl, ILogger<EmployeeController>? logger = null) {
            _employeeControl = employeeControl;
            _logger = logger;
        }

        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> Get() {
            ActionResult<List<Employee>> foundReturn;
            List<Employee>? foundEmployees = await _employeeControl.GetAll();
            if (foundEmployees != null) {
                foundReturn = Ok(foundEmployees);
            } else {
                foundEmployees = new List<Employee>();
                foundReturn = Ok(foundEmployees);
            }
            return foundReturn;
        }


        // POST api/<EmployeeController>
        [HttpPost]

        public async Task<IActionResult> CreateEmployee([FromBody] Employee employeeToCreate) {
            IActionResult foundResult;
            int insertedId = await _employeeControl.Create(employeeToCreate);
            if (insertedId == -1) {
                foundResult = StatusCode(500);

            } else {
                employeeToCreate.EmployeeId = insertedId;
                foundResult = CreatedAtAction(nameof(Get), new { id = insertedId }, employeeToCreate); // 201
            }
            return foundResult;
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(int employeeId, [FromBody] Employee employeeToUpdate) {
            IActionResult foundResult;

            bool isUpdated = await _employeeControl.Update(employeeId, employeeToUpdate);
            if (isUpdated) {
                foundResult = Ok();
            } else {
                foundResult = StatusCode(500);
            }
            return foundResult;
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id) {
            IActionResult result;

            if (id > 0) {
                bool isDeleted = await _employeeControl.Delete(id);
                if (isDeleted) {
                    result = NoContent();
                } else {
                    result = StatusCode(500);
                }
            } else {
                result = BadRequest();
            }
            return result;
        }
    }
}
