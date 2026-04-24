using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class BookingService(IBookingRepository _bookingRepository) : IBookingService
    {
        public async Task<bool> IsTimeSlotAvailableAsync(Guid laundryRoomId, DateOnly bookingDate, TimeSlot timeSlot)
        {
            return !await _bookingRepository.ExistsAsync(laundryRoomId, bookingDate, timeSlot);
        }

        public async Task<Booking> CreateBookingAsync(Guid personId, Guid laundryRoomId, DateOnly bookingDate, TimeSlot timeSlot)
        {
            // check slot first
            bool isAvailable = await IsTimeSlotAvailableAsync(laundryRoomId, bookingDate, timeSlot);

            if (!isAvailable)
            {
                throw new InvalidOperationException("The selected time slot is not available.");
            }

            Booking booking = new Booking
            {
                Id = Guid.NewGuid(),
                PersonId = personId,
                LaundryRoomId = laundryRoomId,
                BookingDate = bookingDate,
                TimeSlot = timeSlot,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return booking;
        }

        public async Task<IEnumerable<Booking>> GetBookingsForRoomByDateAsync(Guid laundryRoomId, DateOnly bookingDate)
        {
            return await _bookingRepository.GetByLaundryRoomAndDateAsync(laundryRoomId, bookingDate);
        }

        public async Task<IEnumerable<Booking>> GetBookingsForPersonAsync(Guid personId)
        {
            return await _bookingRepository.GetByPersonIdAsync(personId);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllWithDetailsAsync();
        }

        public async Task<bool> DeleteBookingAsync(Guid bookingId)
        {
            Booking? booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking is null)
            {
                return false;
            }

            _bookingRepository.Delete(booking);
            await _bookingRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateBookingAsync(Guid bookingId, Guid laundryRoomId, DateOnly bookingDate, TimeSlot timeSlot)
        {
            Booking? booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking is null)
            {
                return false;
            }

            bool isAlreadyTaken = await _bookingRepository.ExistsAsync(laundryRoomId, bookingDate, timeSlot);

            bool sameBooking =
                booking.LaundryRoomId == laundryRoomId &&
                booking.BookingDate == bookingDate &&
                booking.TimeSlot == timeSlot;

            // if same booking no problem
            if (isAlreadyTaken && !sameBooking)
            {
                throw new InvalidOperationException("The selected time slot is already booked.");
            }

            booking.LaundryRoomId = laundryRoomId;
            booking.BookingDate = bookingDate;
            booking.TimeSlot = timeSlot;

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Booking>> GetBookingsForRoomByWeekAsync(Guid laundryRoomId, DateOnly startDate, DateOnly endDate)
        {
            IEnumerable<Booking> bookings = await _bookingRepository.GetByWeekAsync(startDate, endDate);

            // only this room
            return bookings.Where(booking => booking.LaundryRoomId == laundryRoomId);
        }
    }
}
