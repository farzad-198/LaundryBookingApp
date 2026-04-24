using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LaundryRoomRepository : GenericRepository<LaundryRoom>, ILaundryRoomRepository
    {
        public LaundryRoomRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<LaundryRoom?> GetByRoomNameAsync(string roomName)
        {
            return await _dbSet.FirstOrDefaultAsync(room => room.Name == roomName);
        }

        public async Task<IEnumerable<LaundryRoom>> GetAvailableRoomsAsync(DateOnly bookingDate, TimeSlot timeSlot)
        {
            return await _dbSet
                // room with free slot
                .Where(room => !room.Bookings.Any(booking =>
                    booking.BookingDate == bookingDate &&
                    booking.TimeSlot == timeSlot))
                .OrderBy(room => room.Name)
                .ToListAsync();
        }
    }
}
