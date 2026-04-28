using DocGenerator.Application.DTOs.Users;
using DocGenerator.Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocGenerator.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region POST

        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        [AllowAnonymous]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var response = await _userService.CreateUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        #endregion

        #region PUT - PATCH

        /// <summary>
        /// Actualizar al usuario en la web
        /// </summary>
        [HttpPatch("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var response = await _userService.UpdateUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        #endregion
    }
}
