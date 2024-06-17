using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JWT.RequestModel;
using JWT.Services;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAndRegisterRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);
            if (response != null)
            {
                return Ok(response);
            }

            return Unauthorized("Nieprawidłowa nazwa użytkownika lub hasło.");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (response is null)
            {
                return Ok(response);
            }

            return Unauthorized("Nieprawidłowy token.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginAndRegisterRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.RegisterUserAsync(request);
            if (success)
            {
                return Ok("Użytkownik zarejestrowany.");
            }

            return BadRequest("Nazwa użytkownika już istnieje.");
        }
    }
}