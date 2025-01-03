using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Time_slotController : ControllerBase
    {
        private Time_slotService time_slotService;

        public Time_slotController(Time_slotService time_slotService)
        {
            this.time_slotService = time_slotService;
        }

        [HttpGet("/LijstTimeSlots")]
        public IActionResult GetTimeSlots()
        {
            IEnumerable<Time_slot> timeSlots = time_slotService.GetTimeSlots();
            return Ok(timeSlots);
        }

        [HttpGet("/Time_slotViaId/{id}")]
        public IActionResult GetTime_slotId(int id)
        {
            Time_slot time_slot = time_slotService.GetTime_slotId(id);
            if (time_slot == null)
            {
                return NotFound($"Time_slot met id {id} niet gevonden!");
            }

            return Ok(time_slot);
        }
    }
}
