using FitnessAPI.DTO;
using FitnessBL.Enums;
using FitnessBL.Exceptions;
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

        [HttpGet("/LijstMembers")]
        public ActionResult<IEnumerable<Member>> GetMembers()
        {
            try
            {
                IEnumerable<Member> members = memberService.GetMembers();
                return Ok(members);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/MemberViaId/{id}")]
        public IActionResult GetMemberId(int id)
        {
            try
            {
                Member member = memberService.GetMemberId(id);
                return Ok(member);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/MemberViaNaam/{voornaam}/{achternaam}")]
        public IActionResult GetMemberNaam(string voornaam, string achternaam)
        {
            try
            {
                Member member = memberService.GetMemberNaam(voornaam, achternaam);
                return Ok(member);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/MemberToevoegen")]
        public IActionResult AddMember([FromBody] MemberAanmakenDTO memberDTO)
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
            catch (MemberException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("/MemberAanpassen/{voornaam}/{achternaam}")]
        public IActionResult UpdateMember(
            string voornaam,
            string achternaam,
            [FromQuery] DateTime GeboorteDatum,
            [FromQuery] TypeKlant? typeKlant,
            [FromQuery] string? Voornaam = null,
            [FromQuery] string? Achternaam = null,
            [FromQuery] string? Email = null,
            [FromQuery] string? Adres = null,
            [FromQuery] List<string> Interesses = null
        )
        {
            try
            {
                // Haal de gebruiker op uit de database
                Member member = memberService.GetMemberNaam(voornaam, achternaam);

                // Pas alleen de velden aan die zijn meegegeven (niet null)
                if (!string.IsNullOrEmpty(Voornaam))
                    member.FirstName = Voornaam;
                if (!string.IsNullOrEmpty(Achternaam))
                    member.LastName = Achternaam;
                if (!string.IsNullOrEmpty(Email))
                    member.Email = Email;
                if (!string.IsNullOrEmpty(Adres))
                    member.Address = Adres;
                if (GeboorteDatum != new DateTime(0001, 01, 01))
                    member.Birthday = GeboorteDatum;
                if (typeKlant.HasValue)
                    member.MemberType = typeKlant.Value;
                if (Interesses != null && Interesses.Count() != 0)
                    member.Interests = Interesses;

                // Update het record in de database
                memberService.UpdateMember(member);

                return CreatedAtAction(
                    nameof(GetMemberId), // Specify the action name of the "Get" endpoint
                    new { id = member.Member_id }, // Parameter voor de Get eindpoint
                    member // Return the created gebruiker object
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/MemberVerwijderenViaId/{id}")]
        public IActionResult DeleteMember(int id)
        {
            try
            {
                memberService.DeleteMember(id);
                return Ok("De member is succesvol verwijderd!");
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
