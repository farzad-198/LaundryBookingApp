using Domain.Entities;
using Domain.Enums;


namespace Application.Interfaces.Services
{
    public interface ILaundryRoomService
    {
        Task<IEnumerable<LaundryRoom>> GetAllLaundryRoomsAsync();
        Task<LaundryRoom?> GetLaundryRoomByIdAsync(Guid id);
        Task<LaundryRoom?> CreateRoomAsync(LaundryRoom? room);
        Task<IEnumerable<LaundryRoom>> GetAvailableRoomsAsync(DateOnly bookingDate, TimeSlot timeSlot);
    }
}
