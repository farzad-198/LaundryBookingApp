using Domain.Entities;
using Domain.Enums;


namespace Application.Interfaces.Services
{
  public interface IBookingService
    {
        Task<bool> IsTimeSlotAvailableAsync(Guid laundryRoomId, DateOnly bookingDate, TimeSlot timeSlot);
        Task<Booking> CreateBookingAsync(Guid personId, Guid laundryRoomId, DateOnly bookingDate, TimeSlot timeSlot);
        Task<IEnumerable<Booking>> GetBookingsForRoomByDateAsync(Guid laundryRoomId, DateOnly bookingDate);
        Task<IEnumerable<Booking>> GetBookingsForPersonAsync(Guid personId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<bool> DeleteBookingAsync(Guid bookingId);
        Task<bool> UpdateBookingAsync(Guid bookingId, Guid laundryRoomId, DateOnly bookingDate, TimeSlot timeSlot);
        Task<IEnumerable<Booking>> GetBookingsForRoomByWeekAsync(Guid laundryRoomId, DateOnly startDate, DateOnly endDate);
    }
}
