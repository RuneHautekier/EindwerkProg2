using FitnessAPI.DTO;
using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private MemberService memberService;

        public MemberController(MemberService memberService)
        {
            this.memberService = memberService;
        }

        [HttpGet("/MemberViaId/{id}")]
        public IActionResult GetMemberId(int id)
        {
            Member member = memberService.GetMemberId(id);
            if (member == null)
            {
                return NotFound($"Member met id {id} niet gevonden!");
            }

            return Ok(member);
        }

        [HttpPost("/MemberToevoegen")]
        public IActionResult Gebruiker([FromBody] MemberAanmakenDTO memberDTO)
        {
            try
            {
                Member member = new Member(
                    memberDTO.Voornaam,
                    memberDTO.Achternaam,
                    memberDTO.Email,
                    memberDTO.Adres,
                    memberDTO.Geboortedatum,
                    memberDTO.Interesses,
                    memberDTO.TypeKlant
                );

                memberService.AddMember(member);

                return CreatedAtAction(
                    nameof(GetMemberId), // Specify the action name of the "Get" endpoint
                    new { id = member.Member_id }, // Parameter voor de Get eindpoint
                    member // Return the created gebruiker object
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
