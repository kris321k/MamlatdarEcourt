using Microsoft.AspNetCore.Mvc;
using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Services;

namespace MamlatdarEcourt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public UserAuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Validation failed.",
                    Errors = ModelState
                        .Where(x => x.Value!.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                });
            }

            var result = await _authService.RegisterUserAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "User registration failed.",
                    Errors = result.Errors.Select(e => e.Description)
                });
            }

            return StatusCode(201, new { message = "User created successfully" });
        }
    }
}
