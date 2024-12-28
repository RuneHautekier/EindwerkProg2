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

        [HttpPost("/MemberToevoegen")]
        public IActionResult AddMember([FromBody] MemberAanmakenDTO memberDTO)
        {
            try
            {
                List<string> intr = new List<string>();
                if (memberDTO.Intrests.Count >= 1 && !memberDTO.Intrests.Contains("string"))
                {
                    foreach (string str in memberDTO.Intrests)
                    {
                        intr.Add(str);
                    }
                }
                Member member = new Member(
                    memberDTO.FirstName,
                    memberDTO.LastName,
                    memberDTO.Email,
                    memberDTO.Address,
                    memberDTO.Birthday,
                    intr,
                    memberDTO.TypeMember
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

        [HttpPatch("/MemberAanpassen/{id}")]
        public IActionResult UpdateMember(
            int id,
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
                Member member = memberService.GetMemberId(id);

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
                Member member = memberService.GetMemberId(id);
                memberService.DeleteMember(member);
                return Ok("De member is succesvol verwijderd!");
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/GetTrainingSessionsMember/{id}")]
        public IActionResult GetTrainingSessionsMember(int id)
        {
            try
            {
                Member member = memberService.GetMemberId(id);
                IEnumerable<TrainingSession> TrainingSessions =
                    memberService.GetTrainingSessionsMember(member);

                // Maak een lijst om dynamische DTO-objecten op te slaan
                List<dynamic> trainingSessionDTOs = new List<dynamic>();

                // Itereer door alle trainingssessies
                foreach (TrainingSession ts in TrainingSessions)
                {
                    // Controleer of het een RunningSession is
                    if (ts is Runningsession_main runningSession)
                    {
                        // Voeg een DTO-object toe met alleen relevante velden voor een RunningSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Running",
                                Id = runningSession.Runningsession_id,
                                Date = runningSession.Date,
                                Duration = runningSession.Duration,
                                AvgSpeed = runningSession.Avg_speed
                            }
                        );
                    }
                    // Controleer of het een CyclingSession is
                    else if (ts is Cyclingsession cyclingSession)
                    {
                        // Voeg een DTO-object toe met alleen relevante velden voor een CyclingSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Cycling",
                                Id = cyclingSession.Cyclingsession_id,
                                Date = cyclingSession.Date,
                                Duration = cyclingSession.Duration,
                                AvgWatt = cyclingSession.Avg_watt,
                                MaxWatt = cyclingSession.Max_watt,
                                AvgCadence = cyclingSession.Avg_cadence,
                                MaxCadence = cyclingSession.Max_cadence,
                                TrainingsType = cyclingSession.TrainingsType,
                                Comment = cyclingSession.Comment
                            }
                        );
                    }
                }

                return Ok(trainingSessionDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
