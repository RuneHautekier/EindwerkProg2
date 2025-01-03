namespace FitnessAPI.DTO
{
    public class ReservationDTO
    {
        public int ReservationId { get; set; }
        public DateTime Date { get; set; }
        public int MemberId { get; set; }
        public List<TimeslotEquipmentDTO> EquipmentPerTimeslot { get; set; }
    }
}
