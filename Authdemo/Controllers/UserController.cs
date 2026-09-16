using Authdemo.Models;
using Authdemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if(user == null)
            {
                return NotFound(new
                {
                    Message = $"User {id} Not Found"
                });
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var success = await _userService.UpdateUserAsync(id, request);

            if (!success)
            {
                return NotFound(new
                {
                    Message = $"User {id} Not Found"
                });
            }
            return Ok("User Updated Successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);

            if (!success)
            {
                return NotFound(new
                {
                    Message = $"User {id} not found"
                });
            }
            return Ok("User Deleted Successfully");
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(int id) 
        {
            var result = await _userService.DeactivateUserAsync(id);

            if (!result)
            {
                return NotFound(new
                    {
                        Message = $"User {id} not found or already deactivated"
                    }
                );
            }

            return Ok("User Deactivated Successfully");
        }

        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var result = await _userService.ActivateUserAsync(id);

            if (!result)
            {
                return NotFound(new
                    {
                        Message = $"User {id} not found or already active"
                    }
                );
            }

            return Ok($"User {id} activated successfully");
        }
    }
}
