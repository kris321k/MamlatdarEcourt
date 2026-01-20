using Microsoft.AspNetCore.Mvc;
using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Services;
using StackExchange.Redis;

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
            catch
            {
                return StatusCode(500,
                    "Failed to send OTP. Please try again.");
            }
        }

        
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
            [FromBody] VerifyDto otpDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValid =
                _otpService.ValidateOtp(otpDto.SessionId, otpDto.Otp);

            if (!isValid)
                return BadRequest("Invalid or expired OTP.");

            var sessionData =
                _otpService.GetUserData(otpDto.SessionId);

            if (sessionData == null || sessionData.PendingUser == null)
                return BadRequest("OTP session expired.");

            var result =
                await _litigantAuthService
                    .RegisterLigitantAsync(sessionData.PendingUser);

            if (!result.Succeeded)
                return BadRequest("Error creating litigant.");

            var User = await _litigantAuthService.FindLigitantByEmail(sessionData.PendingUser.Email);

            var RoleAssigned = await _litigantAuthService.AddToRoleAsync(User!);

            
            if (!RoleAssigned.Succeeded)
            {
                return BadRequest("Error Assiging the Role");
            }

            _otpService.Clear(otpDto.SessionId);

            return Ok(new
            {
                message = "Litigant registered successfully"
            });
        }

        [HttpPost("verify-login")]

        public async Task<IActionResult> verifyLogin(LoginDto _loginDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var SessionToken = await _litigantAuthService.LoginAsync(_loginDto);

            if (SessionToken == null) return Unauthorized("login credentials not valid");

            return Ok(new { message = SessionToken });


        }
    }

}
