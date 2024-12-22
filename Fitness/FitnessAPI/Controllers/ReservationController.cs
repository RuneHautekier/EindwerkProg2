using FitnessAPI.DTO;
using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private ReservationService reservationService;
        private MemberService memberService;
        private EquipmentService equipmentService;
        private Time_slotService timeSlotService;

        public ReservationController(
            ReservationService reservationService,
            MemberService memberService,
            EquipmentService equipmentService,
            Time_slotService timeSlotService
        )
        {
            this.reservationService = reservationService;
            this.memberService = memberService;
            this.equipmentService = equipmentService;
            this.timeSlotService = timeSlotService;
        }

        [HttpGet("/ReservationViaId/{id}")]
        public IActionResult GetReservationID(int id)
        {
            Reservation reservation = reservationService.GetReservationId(id);
            if (reservation == null)
            {
                return NotFound($"reservation met id {id} niet gevonden!");
            }

            return Ok(reservation);
        }

        [HttpPost("/ReservationAanmaken")]
        public IActionResult CreateReservation([FromBody] ReservationAanmakenDTO reservationDTO)
        {
            try
            {
                Dictionary<Time_slot, Equipment> dic = new Dictionary<Time_slot, Equipment>();
                foreach (TimeslotEquipmentDTO tseDTO in reservationDTO.EquipmentPerTimeslot)
                {
                    Time_slot ts = timeSlotService.GetTime_slotId(tseDTO.Time_slot_id);
                    Equipment e = equipmentService.GetEquipmentId(tseDTO.Equipment_id);
                    dic.Add(ts, e);
                }

                if (dic.Count != 1 && dic.Count != 2)
                {
                    return BadRequest(
                        "Je moet minimaal 1 tijdslot en maximaal 2 tijdsloten reserveren!"
                    );
                }
                else
                {
                    if (dic.Count == 2)
                    {
                        Time_slot ts1 = dic.Keys.First();
                        Time_slot ts2 = dic.Keys.Last();
                        if (
                            ts1.Time_slot_id + 1 != ts2.Time_slot_id
                            || ts1.Time_slot_id - 1 != ts2.Time_slot_id
                        )
                        {
                            return BadRequest("De tijdsloten moeten na elkaar liggen!");
                        }
                    }
                }

                Member member = memberService.GetMemberId(reservationDTO.MemberId);

                Reservation reservation = new Reservation(
                    reservationService.GetNieuwReservationId(),
                    reservationDTO.Date,
                    member,
                    dic
                );

                reservationService.AddReservation(reservation);

                //// Map the created Training entity to a DTO
                //TrainingDTO createdTrainingDTO = new TrainingDTO
                //{
                //    TrainingId = training.Id,
                //    Naam = training.Naam,
                //    Datum = training.Datum,
                //    Type = training.Type,
                //    Status = training.Status,
                //    SchemaID = training.Schema.Id,
                //    GebruikerID = training.Schema.Gebruiker.Id,
                //    GebruikersNaam = training.Schema.Gebruiker.Naam
                //};

                return CreatedAtAction(
                    nameof(GetReservationID),
                    new { id = reservation.Reservation_id }, // Route parameter
                    reservation // Return DTO object
                );
            }
            catch (Exception ex)
            {
                // Log eventueel de fout hier
                return StatusCode(500, $"Er is een fout opgetreden: {ex.Message}");
            }
        }
    }
}
