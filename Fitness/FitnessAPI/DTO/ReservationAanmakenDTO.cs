using FitnessBL.Model;

namespace FitnessAPI.DTO
{
    public class ReservationAanmakenDTO
    {
        public DateTime Date { get; set; }
        public int MemberId { get; set; }
        public List<TimeslotEquipmentDTO> EquipmentPerTimeslot { get; set; }
    }
}
