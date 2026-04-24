using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILaundryRoomRepository _laundryRoomRepository;

        public ReportService(
            IBookingRepository bookingRepository,
            ILaundryRoomRepository laundryRoomRepository)
        {
            _bookingRepository = bookingRepository;
            _laundryRoomRepository = laundryRoomRepository;
        }

        public async Task<string?> GetMostBookedLaundryRoomAsync()
        {
            IEnumerable<Booking> bookings = await _bookingRepository.GetAllAsync();
            IEnumerable<LaundryRoom> rooms = await _laundryRoomRepository.GetAllAsync();

            // count bookings for each room
            var result = bookings
                .GroupBy(booking => booking.LaundryRoomId)
                .Select(group => new
                {
                    LaundryRoomId = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(item => item.Count)
                .FirstOrDefault();

            if (result is null)
            {
                return null;
            }

            LaundryRoom? room = rooms.FirstOrDefault(room => room.Id == result.LaundryRoomId);

            return room?.Name;
        }

        public async Task<string?> GetTopPersonAsync()
        {
            IEnumerable<Booking> bookings = await _bookingRepository.GetAllWithDetailsAsync();

            // get person with more bookings
            var result = bookings
                .Where(booking => booking.Person is not null)
                .GroupBy(booking => booking.PersonId)
                .Select(group => new
                {
                    FullName = group.First().Person!.FullName,
                    Count = group.Count()
                })
                .OrderByDescending(item => item.Count)
                .FirstOrDefault();

            return result?.FullName;
        }

        public async Task<double> GetMostBookedRateForRoomAsync(Guid laundryRoomId, DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException("End date cannot be earlier than start date.");
            }

            IEnumerable<Booking> bookings = await _bookingRepository.GetByWeekAsync(startDate, endDate);

            int totalDays = endDate.DayNumber - startDate.DayNumber + 1;
            int slotsPerDay = 3;
            // total room slots
            int totalAvailableSlots = totalDays * slotsPerDay;

            int bookedSlots = bookings.Count(booking => booking.LaundryRoomId == laundryRoomId);

            if (totalAvailableSlots == 0)
            {
                return 0;
            }

            return (double)bookedSlots / totalAvailableSlots * 100;
        }

        public async Task<double> GetWeeklyOccupancyRateForRoomAsync(Guid laundryRoomId, DateOnly weekStartDate)
        {
            DateOnly weekEndDate = weekStartDate.AddDays(6);

            return await GetMostBookedRateForRoomAsync(laundryRoomId, weekStartDate, weekEndDate);
        }

        public async Task<double> GetTotalOccupancyRateForRoomAsync(Guid laundryRoomId)
        {
            IEnumerable<Booking> allBookings = await _bookingRepository.GetAllAsync();

            List<Booking> roomBookings = allBookings
                .Where(booking => booking.LaundryRoomId == laundryRoomId)
                .OrderBy(booking => booking.BookingDate)
                .ToList();

            if (!roomBookings.Any())
            {
                return 0;
            }

            DateOnly startDate = roomBookings.First().BookingDate;
            DateOnly endDate = roomBookings.Last().BookingDate;

            int totalDays = endDate.DayNumber - startDate.DayNumber + 1;
            int slotsPerDay = 3;
            int totalAvailableSlots = totalDays * slotsPerDay;

            int bookedSlots = roomBookings.Count;

            if (totalAvailableSlots == 0)
            {
                return 0;
            }

            return (double)bookedSlots / totalAvailableSlots * 100;
        }
    }
}
