using Microsoft.AspNetCore.Mvc;
using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Services;
using StackExchange.Redis;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MamlatdarEcourt.Models;


namespace MamlatdarEcourt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CaseController : ControllerBase
    {

        private readonly CaseService _cService;


        public CaseController(CaseService cService)
        {
            _cService = cService;
        }


        [Authorize(Roles = "litigant")]

        [HttpPost("RegisterCase")]

        public async Task<IActionResult> RegisterCase(CaseRegisterDto c)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var ApplicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            try
            {
                var createdCase = await _cService.CreateCaseAsync(c, ApplicantId!);

                return StatusCode(201, createdCase);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "something went wrong creating the case",
                    detail = ex.Message
                });
            }
        }


        [Authorize(Roles = "litigant")]

        [HttpGet("get-all-cases")]

        public async Task<IActionResult> GetAllCases()
        {

            try
            {

                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var Cases = await _cService.FindCaseByApplicantId(UserId!);
                return Ok(Cases);

            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching the cases"
                });
            }
        }
    }
}