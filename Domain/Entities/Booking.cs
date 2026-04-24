using Domain.Enums;


namespace Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;
        public Guid LaundryRoomId { get; set; }
        public LaundryRoom LaundryRoom { get; set; } = null!;
        public DateOnly BookingDate { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public DateTime CreatedAt { get; set; }
       
    }
}
