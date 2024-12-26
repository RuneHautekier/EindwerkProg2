using FitnessAPI.DTO;
using FitnessBL.Enums;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CyclingSessionController : ControllerBase
    {
        private CyclingSessionService cyclingSessionService;
        private MemberService memberService;

        public CyclingSessionController(
            CyclingSessionService cyclingSessionService,
            MemberService memberService
        )
        {
            this.cyclingSessionService = cyclingSessionService;
            this.memberService = memberService;
        }

        [HttpGet("/LijstCyclingSessions")]
        public ActionResult<IEnumerable<Member>> GetMembers()
        {
            try
            {
                IEnumerable<Cyclingsession> cyclingsessions =
                    cyclingSessionService.GetCyclingSessions();
                return Ok(cyclingsessions);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/CyclingSessionViaId/{id}")]
        public IActionResult GetCyclingSessionId(int id)
        {
            try
            {
                Cyclingsession cs = cyclingSessionService.GetCyclingSessionId(id);
                CyclingSessionDTO csDTO = new CyclingSessionDTO
                {
                    CyclingSessionId = cs.Cyclingsession_id,
                    Date = cs.Date,
                    Duration = cs.Duration,
                    AverageWattage = cs.Avg_watt,
                    MaxWattage = cs.Max_watt,
                    AverageCadence = cs.Avg_cadence,
                    MaxCadence = cs.Max_cadence,
                    TrainingsType = cs.TrainingsType,
                    Comment = cs.Comment,
                    MemberId = cs.Member.Member_id,
                    FirstName = cs.Member.FirstName,
                    Lastname = cs.Member.LastName
                };
                return Ok(csDTO);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/CyclingSessionViaMember/{voornaam}/{achternaam}")]
        public IActionResult GetCyclingSessionId(string voornaam, string achternaam)
        {
            try
            {
                Member member = memberService.GetMemberNaam(voornaam, achternaam);
                List<Cyclingsession> css = cyclingSessionService.GetCyclingsessionViaMember(member);

                List<CyclingSessionDTO> csDTOs = new List<CyclingSessionDTO>();

                foreach (Cyclingsession cs in css)
                {
                    CyclingSessionDTO csDTO = new CyclingSessionDTO
                    {
                        CyclingSessionId = cs.Cyclingsession_id,
                        Duration = cs.Duration,
                        AverageWattage = cs.Avg_watt,
                        MaxWattage = cs.Max_watt,
                        AverageCadence = cs.Avg_cadence,
                        MaxCadence = cs.Max_cadence,
                        TrainingsType = cs.TrainingsType,
                        Comment = cs.Comment,
                        MemberId = cs.Member.Member_id,
                        FirstName = cs.Member.FirstName,
                        Lastname = cs.Member.LastName
                    };

                    csDTOs.Add(csDTO);
                }
                return Ok(csDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/CyclingSessionToevoegen")]
        public IActionResult AddCyclingSession([FromBody] CyclingSessionAanmakenDTO csDTO)
        {
            try
            {
                Member member = memberService.GetMemberNaam(csDTO.FirstName, csDTO.Lastname);
                Cyclingsession cs = new Cyclingsession(
                    csDTO.Date,
                    csDTO.Duration,
                    csDTO.AverageWattage,
                    csDTO.MaxWattage,
                    csDTO.AverageCadence,
                    csDTO.MaxCadence,
                    csDTO.TrainingsType,
                    csDTO.Comment,
                    member
                );

                cyclingSessionService.AddCyclingSession(cs);

                return CreatedAtAction(
                    nameof(GetCyclingSessionId), // Specify the action name of the "Get" endpoint
                    new { id = cs.Cyclingsession_id }, // Parameter voor de Get eindpoint
                    csDTO // Return the created gebruiker object
                );
            }
            catch (CyclingSessionException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpPatch("/CyclingSessionAanpassen/{id}")]
        public IActionResult UpdateCyclingSession(
            int id,
            [FromQuery] int duration,
            [FromQuery] int avg_watt,
            [FromQuery] int max_watt,
            [FromQuery] int avg_cadence,
            [FromQuery] int max_cadence,
            [FromQuery] string? trainingtype,
            [FromQuery] string? comment,
            [FromQuery] DateTime Date
        )
        {
            try
            {
                Cyclingsession cs = cyclingSessionService.GetCyclingSessionId(id);

                // Pas alleen de velden aan die zijn meegegeven (niet null)
                if (Date != new DateTime(0001, 01, 01))
                    cs.Date = Date;
                if (!string.IsNullOrEmpty(trainingtype))
                    cs.TrainingsType = trainingtype;
                if (!string.IsNullOrEmpty(comment))
                    cs.Comment = comment;
                if (duration != 0)
                    cs.Duration = duration;
                if (avg_watt != 0)
                    cs.Avg_watt = avg_watt;
                if (max_watt != 0)
                    cs.Max_watt = max_watt;
                if (avg_cadence != 0)
                    cs.Avg_cadence = avg_cadence;
                if (max_cadence != 0)
                    cs.Max_cadence = max_cadence;

                // Update het record in de database
                cyclingSessionService.UpdateCyclingSession(cs);

                return CreatedAtAction(
                    nameof(GetCyclingSessionId),
                    new { id = cs.Cyclingsession_id },
                    cs
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/CyclingSessionVerwijderenViaId/{id}")]
        public IActionResult DeleteCyclingSession(int id)
        {
            try
            {
                cyclingSessionService.DeleteCyclingSession(id);
                return Ok("De cyclingsession is succesvol verwijderd!");
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
