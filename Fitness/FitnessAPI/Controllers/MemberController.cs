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
    }
}
