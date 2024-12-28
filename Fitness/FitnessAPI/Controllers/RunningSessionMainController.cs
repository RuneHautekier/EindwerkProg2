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
    public class RunningSessionMainController : ControllerBase
    {
        private RunningSessionMainService rsmService;
        private MemberService memberService;

        public RunningSessionMainController(
            RunningSessionMainService rsmService,
            MemberService memberService
        )
        {
            this.rsmService = rsmService;
            this.memberService = memberService;
        }

        [HttpGet("/LijstRunningSessionsMain")]
        public ActionResult<IEnumerable<Runningsession_main>> GetRunningSessionsMain()
        {
            try
            {
                IEnumerable<Runningsession_main> rsms = rsmService.GetRunningSessionsMain();
                return Ok(rsms);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/RunningSessionMainViaId/{id}")]
        public IActionResult GetRunningSessionMainId(int id)
        {
            try
            {
                Runningsession_main rsm = rsmService.GetRunningSessionMainId(id);
                RunningSessionMainDTO rsmDTO = new RunningSessionMainDTO
                {
                    RunningSessionId = rsm.Runningsession_id,
                    Date = rsm.Date,
                    Duration = rsm.Duration,
                    AverageSpeed = rsm.Avg_speed,
                    MemberId = rsm.Member.Member_id,
                    FirstName = rsm.Member.FirstName,
                    LastName = rsm.Member.LastName,
                };
                return Ok(rsmDTO);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/RunningSessionsViaMember/{voornaam}/{achternaam}")]
        public IActionResult GetRunningSessionsMainMember(string voornaam, string achternaam)
        {
            try
            {
                Member member = memberService.GetMemberNaam(voornaam, achternaam);
                IEnumerable<Runningsession_main> rsms = rsmService.GetRunningSessionMainMember(
                    member
                );

                List<RunningSessionMainDTO> rsmDTOs = new List<RunningSessionMainDTO>();
                foreach (Runningsession_main rsm in rsms)
                {
                    RunningSessionMainDTO rsmDTO = new RunningSessionMainDTO
                    {
                        RunningSessionId = rsm.Runningsession_id,
                        Date = rsm.Date,
                        Duration = rsm.Duration,
                        AverageSpeed = rsm.Avg_speed,
                        MemberId = rsm.Member.Member_id,
                        FirstName = rsm.Member.FirstName,
                        LastName = rsm.Member.LastName,
                    };

                    rsmDTOs.Add(rsmDTO);
                }

                return Ok(rsmDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/RunningSessionMainToevoegen")]
        public IActionResult AddMember([FromBody] RunningSessionMainAanmakenDTO rsmDTO)
        {
            try
            {
                Member member = memberService.GetMemberNaam(rsmDTO.FirstName, rsmDTO.LastName);
                Runningsession_main rsm = new Runningsession_main(
                    rsmDTO.Date,
                    rsmDTO.Duration,
                    rsmDTO.AverageSpeed,
                    member
                );

                rsmService.AddRunningSessionMain(rsm);

                return CreatedAtAction(
                    nameof(GetRunningSessionMainId), // Specify the action name of the "Get" endpoint
                    new { id = rsm.Runningsession_id }, // Parameter voor de Get eindpoint
                    rsmDTO // Return the created gebruiker object
                );
            }
            catch (RunningSessionMainException ex)
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

        [HttpPatch("/RunningSessionAanpassen/{id}")]
        public IActionResult UpdateRunningSession(
            int id,
            [FromQuery] DateTime date,
            [FromQuery] int duration = 0,
            [FromQuery] float avg_speed = 0
        )
        {
            try
            {
                Runningsession_main rsm = rsmService.GetRunningSessionMainId(id);

                // Pas alleen de velden aan die zijn meegegeven (niet null)


                if (date != new DateTime(0001, 01, 01))
                    rsm.Date = date;
                if (duration != 0)
                    rsm.Duration = duration;
                if (avg_speed != 0)
                    rsm.Avg_speed = avg_speed;

                // Update het record in de database
                rsmService.UpdateRunningSessionMain(rsm);

                RunningSessionMainDTO rsmDTO = new RunningSessionMainDTO
                {
                    RunningSessionId = rsm.Runningsession_id,
                    Date = rsm.Date,
                    Duration = rsm.Duration,
                    AverageSpeed = avg_speed,
                    MemberId = rsm.Member.Member_id,
                    FirstName = rsm.Member.FirstName,
                    LastName = rsm.Member.LastName,
                };

                return CreatedAtAction(
                    nameof(GetRunningSessionMainId), // Specify the action name of the "Get" endpoint
                    new { id = rsm.Runningsession_id }, // Parameter voor de Get eindpoint
                    rsmDTO // Return the created gebruiker object
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/RunningSessionVerwijderen/{id}")]
        public IActionResult DeleteRunningSession(int id)
        {
            try
            {
                Runningsession_main rsm = rsmService.GetRunningSessionMainId(id);
                rsmService.DeleteRunningSessionMain(rsm);
                return Ok("De RunningSession is succesvol verwijderd!");
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
