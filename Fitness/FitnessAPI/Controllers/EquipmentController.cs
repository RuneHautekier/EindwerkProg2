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

        public EquipmentController(EquipmentService equipmentService)
        {
            this.equipmentService = equipmentService;
        }

        [HttpGet("/LijstEquipment")]
        public ActionResult<IEnumerable<Equipment>> GetEquipment()
        {
            try
            {
                IEnumerable<Equipment> equipments = equipmentService.GetEquipment();

                return Ok(equipments);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpGet("/EquipmentsViaType/{type}")]
        public IActionResult GetEquipmentType(string type)
        {
            try
            {
                IEnumerable<Equipment> equipments = equipmentService.GetEquipmentsType(type);
                return Ok(equipments);
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

        [HttpPatch("/EquipmentAanpassen/{id}")]
        public IActionResult UpdateEquipment(int id, [FromQuery] string device_type)
        {
            try
            {
                Equipment equipment = equipmentService.GetEquipmentId(id);
                equipment.Device_type = device_type;
                // Update het record in de database
                equipmentService.UpdateEquipment(equipment);

                return CreatedAtAction(
                    nameof(GetEquipmentId), // Specify the action name of the "Get" endpoint
                    new { id = equipment.Equipment_id }, // Parameter voor de Get eindpoint
                    equipment // Return the created gebruiker object
                ); // 204 No Content
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Er is een fout opgetreden: {ex.Message}");
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
                equipmentService.EquipmentPlaatsOnderhoud(equipment);
                return Ok(
                    $"Equipment met id {equipment.Equipment_id} is succesvol in onderhoud geplaatst!"
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
                equipmentService.EquipmentVerwijderOnderhoud(equipment);
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
