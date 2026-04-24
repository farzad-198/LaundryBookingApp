using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class LaundryRoomService : ILaundryRoomService
    {
        private readonly ILaundryRoomRepository _laundryRoomRepository;

        public LaundryRoomService(ILaundryRoomRepository laundryRoomRepository)
        {
            _laundryRoomRepository = laundryRoomRepository;
        }

        public async Task<IEnumerable<LaundryRoom>> GetAllLaundryRoomsAsync()
        {
            return await _laundryRoomRepository.GetAllAsync();
        }

        public async Task<LaundryRoom?> GetLaundryRoomByIdAsync(Guid id)
        {
            return await _laundryRoomRepository.GetByIdAsync(id);
        }

        public async Task<LaundryRoom?> CreateRoomAsync(LaundryRoom? room)
        {
            if (room is null)
            {
                return null;
            }

            room.Id = Guid.NewGuid();

            await _laundryRoomRepository.AddAsync(room);
            await _laundryRoomRepository.SaveChangesAsync();

            return room;
        }

        public async Task<IEnumerable<LaundryRoom>> GetAvailableRoomsAsync(DateOnly bookingDate, TimeSlot timeSlot)
        {
            return await _laundryRoomRepository.GetAvailableRoomsAsync(bookingDate, timeSlot);
        }
    }
}
