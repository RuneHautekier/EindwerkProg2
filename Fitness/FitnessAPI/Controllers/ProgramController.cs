using FitnessAPI.DTO;
using FitnessBL.Enums;
using FitnessBL.Exceptions;
using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;
using ProgramBL = FitnessBL.Model.Program;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private ProgramService programService;
        private MemberService memberService;

        public ProgramController(ProgramService programService, MemberService memberService)
        {
            this.programService = programService;
            this.memberService = memberService;
        }

        [HttpGet("/ProgramViaProgramCode/{programCode}")]
        public IActionResult GetProgramCode(string programCode)
        {
            try
            {
                ProgramBL program = programService.GetProgramCode(programCode);
                return Ok(program);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/ProgrammaToevoegen")]
        public IActionResult AddProgram([FromBody] ProgramAanmakenDTO programDTO)
        {
            try
            {
                ProgramBL program = new ProgramBL(
                    programDTO.ProgramCode,
                    programDTO.Name,
                    programDTO.Target,
                    programDTO.Startdate,
                    programDTO.Max_members
                );

                programService.AddProgram(program);

                return CreatedAtAction(
                    nameof(GetProgramCode), // Specify the action name of the "Get" endpoint
                    new { programCode = program.ProgramCode }, // Parameter voor de Get eindpoint
                    program // Return the created gebruiker object
                );
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("/ProgramAanpassen/{programCode}")]
        public IActionResult UpdateMember(
            string programCode,
            [FromQuery] DateTime StartDate,
            [FromQuery] string? Name = null,
            [FromQuery] string? Target = null,
            [FromQuery] int maxMembers = 0
        )
        {
            try
            {
                // Haal de gebruiker op uit de database
                ProgramBL program = programService.GetProgramCode(programCode);

                // Pas alleen de velden aan die zijn meegegeven (niet null)
                if (!string.IsNullOrEmpty(Name))
                    program.Name = Name;
                if (!string.IsNullOrEmpty(Target))
                    program.Target = Target;

                if (StartDate != new DateTime(0001, 01, 01))
                    program.Startdate = StartDate;
                if (maxMembers != 0)
                    program.Max_members = maxMembers;

                // Update het record in de database
                programService.UpdateProgram(program);

                return CreatedAtAction(
                    nameof(GetProgramCode), // Specify the action name of the "Get" endpoint
                    new { programCode = program.ProgramCode }, // Parameter voor de Get eindpoint
                    program // Return the created gebruiker object
                );
            }
            catch (ProgramException ex)
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
