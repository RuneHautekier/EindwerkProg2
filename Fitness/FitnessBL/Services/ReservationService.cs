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

        public ReservationService(
            IReservationRepo reservationRepo,
            MemberService memberService,
            EquipmentService equipmentService
        )
        {
            this.reservationRepo = reservationRepo;
            this.memberService = memberService;
            this.equipmentService = equipmentService;
        }

        public Reservation GetReservationId(int id)
        {
            if (id <= 0)
                throw new ServiceException(
                    "ReservationService - GetReservationId - Voer een geldig id in >0!"
                );

            Reservation reservation = reservationRepo.GetReservationId(id);
            if (reservation == null)
                throw new ServiceException(
                    "ReservationService - GetReservationId - Er is geen Reservation met dit Id "
                );

            return reservation;
        }

        public int GetNieuwReservationId()
        {
            return reservationRepo.GetNieuwReservationId();
        }

        public Reservation AddReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ServiceException("Reservation - Reservatie is null");

            // Controleerd of een reservatie 1 of 2 tijdsloten heeft
            if (
                reservation.TimeslotEquipment.Count() < 1
                || reservation.TimeslotEquipment.Count() > 2
            )
            {
                throw new ReservationException(
                    "Je moet minimaal 1 tijdslot en maximaal 2 tijdsloten reserveren!"
                );
            }

            // Controleerd of de tijdsloten naast elkaar liggen
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

            // Controleerd of er een Equipment gebruikt wordt die in onderhoud zit
            foreach (Equipment e in reservation.TimeslotEquipment.Values)
            {
                if (equipmentService.EquipmentInOnderhoud(e))
                {
                    throw new EquipmentException(
                        $"Equipment met id {e.Equipment_id} zit momenteel in onderhoud!"
                    );
                }
            }

            // Controleerd of een gebruiker nog TimeSlots over heeft om een reservatie te reserveren
            int geboekteTijdsloten = memberService.GetAantalGeboekteTijdsloten(
                reservation.Member,
                reservation.Date
            );

            if (geboekteTijdsloten + reservation.TimeslotEquipment.Count() > 4)
            {
                throw new MemberException(
                    "Een member mag maximaal 4 TimeSlots per dag reserveren."
                );
            }

            // Controleerd of de TimeSlots in dezelfde dagindeling vallen
            bool sameSession =
                reservation
                    .TimeslotEquipment.Keys.Select(slot => slot.Part_of_day)
                    .Distinct()
                    .Count() == 1;

            if (!sameSession)
            {
                throw new Time_SlotException(
                    "Alle Time_slots moeten in dezelfde dagindeling vallen."
                );
            }

            //Controleerd of reservatie al bestaat.
            reservationRepo.CheckIfReservationExists(reservation);
            reservationRepo.AddReservation(reservation);
            return reservation;
        }

        public void UpdateReservationsWithNewEquipment(Equipment equipmentInOnderhoud)
        {
            // 1. Haal toekomstige reserveringen op waarin het equipment voorkomt
            IEnumerable<Reservation> reservations =
                equipmentService.GetFutureReservationsForEquipment(equipmentInOnderhoud);

            Equipment oudEquipment = equipmentService.GetEquipmentId(
                equipmentInOnderhoud.Equipment_id
            );
            Equipment alternativeEquipment = null;

            //IEnumerable<Reservation> oudeReservations = reservations;

            foreach (Reservation reservation in reservations)
            {
                Reservation oudeReservation = reservation;

                foreach (Time_slot timeslot in reservation.TimeslotEquipment.Keys.ToList())
                {
                    if (
                        reservation.TimeslotEquipment[timeslot].Equipment_id
                        == equipmentInOnderhoud.Equipment_id
                    )
                    {
                        // 2. Zoek een alternatief equipment dat beschikbaar is op dit tijdslot
                        alternativeEquipment = equipmentService.GetAvailableEquipment(
                            reservation.Date,
                            timeslot,
                            equipmentInOnderhoud.Device_type
                        );

                        if (alternativeEquipment == null)
                        {
                            throw new ServiceException(
                                $"Geen beschikbaar alternatief equipment gevonden voor tijdslot {timeslot}!"
                            );
                        }

                        // 3. Update het equipment in de reservering
                        reservation.TimeslotEquipment[timeslot] = alternativeEquipment;
                    }
                }

                // 4. Sla de gewijzigde reservering op
                reservationRepo.DeleteReservation(oudeReservation);
                reservationRepo.AddReservation(reservation);
            }
        }

        public void UpdateReservationTimeSlots(
            Reservation reservation,
            Reservation geUpdateReservation
        )
        {
            if (reservation == null)
                throw new ServiceException(
                    "ReservationService - UpdateReservationTimeSlots - reservation is null!"
                );
            if (!reservationRepo.IsReservationId(reservation))
                throw new ServiceException(
                    "ReservationService - UpdateReservationTimeSlots - Er is geen reservation met dit id!"
                );

            if (geUpdateReservation == null)
                throw new ServiceException(
                    "ReservationService - UpdateReservationTimeSlots - reservation is null!"
                );

            reservationRepo.DeleteReservation(reservation);
            reservationRepo.AddReservation(geUpdateReservation);
        }

        public void DeleteReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ServiceException(
                    "ReservationService - UpdateReservationTimeSlots - reservation is null!"
                );

            if (!reservationRepo.IsReservationId(reservation))
                throw new ServiceException(
                    "ReservationService - DeleteReservation - Reservation bestaat niet met dit id!"
                );
            reservationRepo.DeleteReservation(reservation);
        }
    }
}
