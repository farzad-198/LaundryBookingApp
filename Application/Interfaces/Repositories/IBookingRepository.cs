using Domain.Entities;
using Domain.Enums;


namespace Application.Interfaces.Repositories
{
    public interface IBookingRepository: IGenericRepository<Booking>
    {
        Task<bool> ExistsAsync(Guid laundryRoomId,DateOnly bookingDate, TimeSlot timeSlot);
        Task<IEnumerable<Booking>> GetByLaundryRoomAndDateAsync(Guid laundryRoomId, DateOnly bookingDate);
        Task<IEnumerable<Booking>> GetByPersonIdAsync(Guid personId);
        Task<IEnumerable<Booking>> GetByWeekAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Booking>> GetAllWithDetailsAsync();
        Task<IEnumerable<Booking>> GetByLaundryRoomIdAsync(Guid laundryRoomId);
    }
}
