using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserControl _userControl;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserControl userControl, ILogger<UserController> logger)
        {
            _userControl = userControl;
            _logger = logger;
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<bool>> CreateUser([FromBody] User userToCreate)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateUser received invalid model");
                return BadRequest(ModelState);
            }

            userToCreate.UserId ??= Guid.NewGuid().ToString();

            try
            {
                bool insertedOk = await _userControl.Create(userToCreate);

                if (insertedOk)
                {
                    _logger.LogInformation("User created with ID: {UserId}", userToCreate.UserId);
                    return Ok(true);
                } else
                {
                    _logger.LogWarning("User creation failed for email: {Email}", userToCreate.Email);
                    return StatusCode(500, "User creation failed");
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating user");
                return StatusCode(500, "Internal server error");
            }
        }


        // GET api/<UserController>/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(string id)
        {
            var foundUser = await _userControl.Get(id);
            if (foundUser != null)
            {
                return Ok(foundUser);
            } else
            {
                return NoContent();
            }
        }

        // PUT api/<UserController>/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Put(string id, [FromBody] User userToUpdate)
        {
            if (userToUpdate == null) return BadRequest();

            bool isUpdated = await _userControl.Update(userToUpdate);

            if (isUpdated)
            {
                return Ok(true);
            } else
            {
                return BadRequest("Update failed");
            }
        }

        // GET api/user/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("GetByEmail called with empty email");
                return BadRequest("Email is required");
            }

            var user = await _userControl.GetByEmail(email);

            if (user == null)
            {
                _logger.LogWarning("No user found with email: {Email}", email);
                return NotFound("Bruger ikke fundet");
            }

            _logger.LogInformation("User found with email: {Email}", email);
            return Ok(user);
        }

    }
}