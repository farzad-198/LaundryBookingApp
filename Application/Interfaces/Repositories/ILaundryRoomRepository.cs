using Domain.Entities;
using Domain.Enums;


namespace Application.Interfaces.Repositories
{
    public interface ILaundryRoomRepository : IGenericRepository<LaundryRoom>
    {
        Task<LaundryRoom?> GetByRoomNameAsync(string roomName);
        Task<IEnumerable<LaundryRoom>> GetAvailableRoomsAsync(DateOnly bookingDate, TimeSlot timeSlot);
    }
}
