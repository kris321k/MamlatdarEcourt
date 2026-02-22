using Microsoft.AspNetCore.Mvc;
using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Services;
using StackExchange.Redis;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MamlatdarEcourt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAuthController : ControllerBase
    {
        private readonly LitigantAuthService _litigantAuthService;
        private readonly OtpService _otpService;

        public UserAuthController(
            LitigantAuthService litigantAuthService,
            OtpService otpService)
        {
            _litigantAuthService = litigantAuthService;
            _otpService = otpService;
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] UserRegister userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser =
                await _litigantAuthService.FindLigitantByEmail(userDto.Email);

            if (existingUser != null)
                return Conflict("User with this email already exists.");

            try
            {
                var sessionId =
                    await _otpService.SendOtpAsync(userDto.Email, userDto);

                return Ok(new
                {
                    message = "OTP sent successfully",
                    otpSessionId = sessionId
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "Failed to send the otp" });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyDto otpDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValid =
                _otpService.ValidateOtp(otpDto.otpSessionId, otpDto.otp);

            if (!isValid)
            {
                Console.WriteLine("invalid otp");

                return BadRequest("Invalid or expired OTP.");
            }

            var sessionData =
                _otpService.GetUserData(otpDto.otpSessionId);

            if (sessionData == null || sessionData.PendingUser == null)
            {
                Console.WriteLine("OTP session expired");
                return BadRequest("OTP session expired.");

            }

            var result =
                await _litigantAuthService
                    .RegisterLigitantAsync(sessionData.PendingUser);

            Console.WriteLine(sessionData.PendingUser.Email);
            Console.WriteLine(sessionData.PendingUser.DOB);

            if (!result.Succeeded)
            {
                Console.WriteLine("Not succeeded creating the litigant");
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { message = "Error creating litigant.", errors });
            }
            var user =
                await _litigantAuthService
                    .FindLigitantByEmail(sessionData.PendingUser.Email);

            var roleAssigned =
                await _litigantAuthService.AddToRoleAsync(user!);

            if (!roleAssigned.Succeeded)
                return BadRequest("Error assigning the role");

            _otpService.Clear(otpDto.otpSessionId);

            return Ok(new
            {
                message = "Litigant registered successfully"
            });
        }


        [HttpPost("verify-login")]
        public async Task<IActionResult> VerifyLogin([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sessionToken =
                await _litigantAuthService.LoginAsync(loginDto);

            if (sessionToken == null)
                return Unauthorized("Login credentials not valid");

            Console.WriteLine(sessionToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(2),
            };

            Response.Cookies.Append(
                "SecureSessionToken",
                sessionToken,
                cookieOptions);

            return Ok(new { message = "Login successful" });
        }

        [Authorize]

        [HttpGet("private")]

        public async Task<IActionResult> PrivateEndpoint()
        {

            var name = User.Identity?.Name;
        
            return Ok(new
            {
                name = User.Identity?.Name,
                litigant = _litigantAuthService.FindLigitantByEmail(name!),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }
    }
}
