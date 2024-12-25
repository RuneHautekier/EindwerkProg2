using FitnessAPI.DTO;
using FitnessBL.Enums;
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

        [HttpPatch("/MemberAanpassen/{voornaam}/{achternaam}")]
        public IActionResult UpdateGebruikerSportief(
            string voornaam,
            string achternaam,
            [FromQuery] DateTime GeboorteDatum,
            [FromQuery] TypeKlant typeKlant,
            [FromQuery] string? Voornaam = null,
            [FromQuery] string? Achternaam = null,
            [FromQuery] string? Email = null,
            [FromQuery] string? Adres = null
        )
        {
            try
            {
                // Haal de gebruiker op uit de database
                Member member = memberService.GetMemberNaam(voornaam, achternaam);
                if (member == null)
                    return NotFound($"Gebruiker met naam {voornaam} {achternaam} niet gevonden.");

                // Pas alleen de velden aan die zijn meegegeven (niet null)
                if (!string.IsNullOrEmpty(Voornaam))
                    member.FirstName = Voornaam;
                if (!string.IsNullOrEmpty(Achternaam))
                    member.LastName = Achternaam;
                if (!string.IsNullOrEmpty(Email))
                    member.Email = Email;
                if (!string.IsNullOrEmpty(Adres))
                    member.Address = Adres;
                if (GeboorteDatum != null)
                    member.Birthday = GeboorteDatum;
                if (typeKlant != null)
                    member.MemberType = typeKlant;

                // Update het record in de database
                memberService.UpdateMember(member);

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Er is een fout opgetreden: {ex.Message}");
            }
        }
    }
}
