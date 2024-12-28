using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;

namespace FitnessBL.Services
{
    public class MaintenanceService
    {
        private readonly EquipmentService equipmentService;
        private readonly ReservationService reservationService;

        public MaintenanceService(
            EquipmentService equipmentService,
            ReservationService reservationService
        )
        {
            this.equipmentService = equipmentService;
            this.reservationService = reservationService;
        }

        public void PlaatsEquipmentOnderhoudMetReserveringUpdate(Equipment equipment)
        {
            using (var transaction = equipmentService.equipmentRepo.BeginTransaction()) // Start de transactie
            {
                try
                {
                    // Stap 3: Haal toekomstige reserveringen op voor het equipment
                    IEnumerable<Reservation> reservationList =
                        reservationService.GetFutureReservationsForEquipment(
                            equipment.Equipment_id
                        );

                    // Stap 4: Update reserveringen met nieuw equipment
                    foreach (Reservation reservation in reservationList)
                    {
                        reservationService.UpdateReservationsWithNewEquipment(equipment);
                    }

                    //equipmentService.EquipmentPlaatsOnderhoud(equipment);
                    // Commit de transactie
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback de transactie bij een fout
                    transaction.Rollback();
                    throw new ServiceException(
                        "Fout bij het plaatsen van equipment in onderhoud en het bijwerken van reserveringen",
                        ex
                    );
                }
            }
        }
    }
}
