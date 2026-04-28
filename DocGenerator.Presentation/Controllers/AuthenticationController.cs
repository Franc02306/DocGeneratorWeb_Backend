using DocGenerator.Application.DTOs.Authentications;
using DocGenerator.Application.Services.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocGenerator.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Valida las credencales y determinar si el usuario puede iniciar sesión en la web
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
        {
            var response = await _authenticationService.LoginUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
