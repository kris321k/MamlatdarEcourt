using Microsoft.AspNetCore.Mvc;
using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Services;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;

namespace MamlatdarEcourt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class UserAuthController : ControllerBase
    {
        private readonly LitigantAuthService _litigantAuthSerive;

        public UserAuthController(LitigantAuthService litigantAuthService)
        {
            _litigantAuthSerive = litigantAuthService;
        }


        [HttpPost("RegisterLitigant")]

        public async Task<IActionResult> registerLitigant([FromBody] UserRegister userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (await _litigantAuthSerive.FindLigitantByEmail(userDto.Email) != null)
            {
                return Conflict(new { Error = "The email already exists" });
            }

            var result = await _litigantAuthSerive.RegisterLigitantAsync(userDto);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(a => new
                {
                    a.Code,
                    a.Description
                });
            }

            return Ok(new { Message = "the object is created succesfully" });
        }

    }

}
