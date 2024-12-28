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
    public class EquipmentController : ControllerBase
    {
        private EquipmentService equipmentService;
        private ReservationService reservationService;
        private MaintenanceService maintenanceService;

        public EquipmentController(
            EquipmentService equipmentService,
            ReservationService reservationService,
            MaintenanceService maintenanceService
        )
        {
            this.equipmentService = equipmentService;
            this.reservationService = reservationService;
            this.maintenanceService = maintenanceService;
        }

        [HttpGet("/EquipmentViaId/{id}")]
        public IActionResult GetEquipmentId(int id)
        {
            try
            {
                Equipment equipment = equipmentService.GetEquipmentId(id);

                return Ok(equipment);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/EquipmentToevoegen")]
        public IActionResult AddEquipment([FromBody] EquipmentAanmakenDTO equipmentDTO)
        {
            try
            {
                Equipment equipment = new Equipment(equipmentDTO.device_type);
                equipmentService.AddEquipment(equipment);

                return CreatedAtAction(
                    nameof(GetEquipmentId), // Specify the action name of the "Get" endpoint
                    new { id = equipment.Equipment_id }, // Parameter voor de Get eindpoint
                    equipment // Return the created gebruiker object
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/EquipmentVerwijderenViaId/{id}")]
        public IActionResult DeleteEquipment(int id)
        {
            try
            {
                Equipment equipment = equipmentService.GetEquipmentId(id);
                equipmentService.DeleteEquipment(equipment);
                return Ok($"Equipment met id {id} is succesvol verwijderd!");
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/EquipmentInOnderhoudPlaatsen")]
        public IActionResult EquipmentPlaatsOnderhoud(
            [FromBody] EquipmentOnderhoudDTO equipmentOnderhoudDTO
        )
        {
            try
            {
                Equipment equipment = equipmentService.GetEquipmentId(
                    equipmentOnderhoudDTO.EquipmentId
                );
                maintenanceService.PlaatsEquipmentOnderhoudMetReserveringUpdate(equipment);
                return Ok(
                    $"Equipment met id {equipmentOnderhoudDTO.EquipmentId} is succesvol in onderhoud geplaatst!"
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/EquipmentUitOnderhoudVerwijderen")]
        public IActionResult EquipmentVerwijderOnderhoud(
            [FromBody] EquipmentOnderhoudDTO equipmentOnderhoudDTO
        )
        {
            try
            {
                Equipment equipment = equipmentService.GetEquipmentId(
                    equipmentOnderhoudDTO.EquipmentId
                );
                equipmentService.EquipmentVerwijderOnderhoud(equipment, DateTime.Now);
                return Ok(
                    $"Equipment met id {equipment.Equipment_id} is succesvol uit onderhoud verwijderd!"
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
