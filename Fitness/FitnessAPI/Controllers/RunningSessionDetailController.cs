using FitnessAPI.DTO;
using FitnessBL.Exceptions;
using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunningSessionDetailController : ControllerBase
    {
        private RunningSessionMainService rsmService;
        private RunningSessionDetailService rsdService;

        public RunningSessionDetailController(
            RunningSessionMainService rsmService,
            RunningSessionDetailService rsdService
        )
        {
            this.rsmService = rsmService;
            this.rsdService = rsdService;
        }

        [HttpGet("/LijstRunningSessionDetails")]
        public ActionResult<IEnumerable<Runningsession_detail>> GetRunningSessionsDetails()
        {
            try
            {
                IEnumerable<Runningsession_detail> rsds = rsdService.GetRunningSessionsDetails();
                return Ok(rsds);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/RunningSessionDetailsViaId/{id}")]
        public IActionResult GetRunningSessionDetailsId(int id)
        {
            try
            {
                IEnumerable<Runningsession_detail> rsds = rsdService.GetRunningSessionDetailsId(id);
                List<RunningSessionDetailDTO> rsdDTOs = new List<RunningSessionDetailDTO>();

                foreach (Runningsession_detail rsd in rsds)
                {
                    RunningSessionMainDTO rsmDTO = new RunningSessionMainDTO
                    {
                        RunningSessionId = rsd.MainSession.Runningsession_id,
                        Date = rsd.MainSession.Date,
                        Duration = rsd.MainSession.Duration,
                        AverageSpeed = rsd.MainSession.Avg_speed,
                        MemberId = rsd.MainSession.Member.Member_id,
                        FirstName = rsd.MainSession.Member.FirstName,
                        LastName = rsd.MainSession.Member.LastName,
                    };

                    RunningSessionDetailDTO rsdDTO = new RunningSessionDetailDTO
                    {
                        MainSession = rsmDTO,
                        SequalNumber = rsd.Seq_nr,
                        IntervalTime = rsd.Interval_time,
                        IntervalSpeed = rsd.Interval_speed,
                    };

                    rsdDTOs.Add(rsdDTO);
                }

                return Ok(rsdDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/RunningSessionDetailsToevoegen/")]
        public IActionResult AddRunningSessionDetails(
            [FromBody] List<RunningSessionDetailAanmakenDTO> rsdDTOs
        )
        {
            try
            {
                Runningsession_main rsm = rsmService.GetRunningSessionMainId(
                    rsdDTOs.First().RunningSessionId
                );
                List<Runningsession_detail> rsds = new List<Runningsession_detail>();
                int seqNr = 1;
                foreach (RunningSessionDetailAanmakenDTO rsdDTO in rsdDTOs)
                {
                    Runningsession_detail rsd = new Runningsession_detail(
                        rsm,
                        seqNr,
                        rsdDTO.IntervalTime,
                        rsdDTO.IntervalSpeed
                    );
                    rsds.Add(rsd);
                    seqNr++;
                }

                rsdService.AddRunningSessionDetails(rsds);

                return CreatedAtAction(
                    nameof(GetRunningSessionDetailsId), // Specify the action name of the "Get" endpoint
                    new { id = rsm.Runningsession_id }, // Parameter voor de Get eindpoint
                    rsdDTOs // Return the created gebruiker object
                );
            }
            catch (RunningSessionMainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (RunningSessionDetailException ex)
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
    }
}
