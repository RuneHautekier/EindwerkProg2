using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;

namespace FitnessBL.Services
{
    public class ReservationService
    {
        private IReservationRepo reservationRepo;
        private MemberService memberService;
        private EquipmentService equipmentService;
        private Time_slotService timeSlotService;

        public ReservationService(
            IReservationRepo reservationRepo,
            MemberService memberService,
            EquipmentService equipmentService,
            Time_slotService timeSlotService
        )
        {
            this.reservationRepo = reservationRepo;
            this.memberService = memberService;
            this.equipmentService = equipmentService;
            this.timeSlotService = timeSlotService;
        }

        public Reservation GetReservationId(int id)
        {
            try
            {
                return reservationRepo.GetReservationId(id);
            }
            catch (Exception ex)
            {
                throw new ServiceException("ReservationService - GetReservationId");
            }
        }

        public int GetNieuwReservationId()
        {
            try
            {
                return reservationRepo.GetNieuwReservationId();
            }
            catch (Exception ex)
            {
                throw new ServiceException("ReservationService - GetNieuwReservationId");
            }
        }

        public Reservation AddReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ServiceException("Reservation - Reservatie is null");

            if (
                reservation.TimeslotEquipment.Count() < 1
                || reservation.TimeslotEquipment.Count() > 2
            )
            {
                throw new ReservationException(
                    "Je moet minimaal 1 tijdslot en maximaal 2 tijdsloten reserveren!"
                );
            }

            if (reservation.TimeslotEquipment.Count() == 2)
            {
                int verschil = Math.Abs(
                    reservation.TimeslotEquipment.Keys.First().Time_slot_id
                        - reservation.TimeslotEquipment.Keys.Last().Time_slot_id
                );
                if (verschil != 1)
                {
                    throw new Time_SlotException("De tijdsloten moeten na elkaar liggen!");
                }
            }

            reservationRepo.CheckIfReservationExists(reservation);
            reservationRepo.AddReservation(reservation);
            return reservation;
        }
    }
}
