using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private EquipmentService equipmentService;

        public EquipmentController(EquipmentService equipmentService)
        {
            this.equipmentService = equipmentService;
        }

        [HttpGet("/EquipmentViaId/{id}")]
        public IActionResult GetEquipmentId(int id)
        {
            Equipment equipment = equipmentService.GetEquipmentId(id);
            if (equipment == null)
            {
                return NotFound($"equipment met id {id} niet gevonden!");
            }

            return Ok(equipment);
        }
    }
}
