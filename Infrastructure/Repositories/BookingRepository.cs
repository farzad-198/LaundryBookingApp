using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(Guid laundryRoomId, DateOnly bookingDate, TimeSlot timeSlot)
        {
            return await _dbSet.AnyAsync(booking =>
                booking.LaundryRoomId == laundryRoomId &&
                booking.BookingDate == bookingDate &&
                booking.TimeSlot == timeSlot);
        }

        public async Task<IEnumerable<Booking>> GetByLaundryRoomAndDateAsync(Guid laundryRoomId, DateOnly bookingDate)
        {
            return await _dbSet
                // load person and room too
                .Include(booking => booking.Person)
                .Include(booking => booking.LaundryRoom)
                .Where(booking =>
                    booking.LaundryRoomId == laundryRoomId &&
                    booking.BookingDate == bookingDate)
                .OrderBy(booking => booking.TimeSlot)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByPersonIdAsync(Guid personId)
        {
            return await _dbSet
                // load room name too
                .Include(booking => booking.Person)
                .Include(booking => booking.LaundryRoom)
                .Where(booking => booking.PersonId == personId)
                .OrderByDescending(booking => booking.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByWeekAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet
                .Include(booking => booking.Person)
                .Include(booking => booking.LaundryRoom)
                .Where(booking =>
                    booking.BookingDate >= startDate &&
                    booking.BookingDate <= endDate)
                .OrderBy(booking => booking.BookingDate)
                .ThenBy(booking => booking.TimeSlot)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(booking => booking.Person)
                .Include(booking => booking.LaundryRoom)
                .OrderBy(booking => booking.BookingDate)
                .ThenBy(booking => booking.TimeSlot)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByLaundryRoomIdAsync(Guid laundryRoomId)
        {
            return await _dbSet
                .Include(booking => booking.Person)
                .Include(booking => booking.LaundryRoom)
                .Where(booking => booking.LaundryRoomId == laundryRoomId)
                .ToListAsync();
        }
    }
}
