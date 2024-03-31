using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {

        private readonly IUserControl _userControl;

        public UserController(IUserControl userControl) {
            _userControl = userControl;
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<bool>> CreateUser([FromBody] User userToCreate) {
            ActionResult<bool> foundResult;
            bool insertedOk = false;

            if (userToCreate != null) {
                insertedOk = await _userControl.Create(userToCreate);

                if (insertedOk == true) {
                    foundResult = Ok(insertedOk);
                } else {
                    return BadRequest();
                }
            } else {
                foundResult = new BadRequestResult();
            }

            return foundResult;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(string id) {
            ActionResult<User> foundReturn;

            User? foundUser = await _userControl.Get(id);

            if (foundUser != null) {
                foundReturn = Ok(foundUser);
            } else {
                foundReturn = new StatusCodeResult(204);
            }
            return foundReturn;
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Put(string id, [FromBody] User userToUpdate) {
            ActionResult<bool > foundResult;
            bool isUpdated = false;
            if (userToUpdate != null) {
                isUpdated = await _userControl.Update(userToUpdate);

                if (isUpdated == true) {
                    foundResult = Ok(isUpdated);
                } else {
                    return BadRequest();
                }
            } else {
                foundResult = new BadRequestResult();
            }
            return foundResult;
        }
    }
}
