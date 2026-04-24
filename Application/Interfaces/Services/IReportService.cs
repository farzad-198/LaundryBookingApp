using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<string?> GetMostBookedLaundryRoomAsync();
        Task<string?> GetTopPersonAsync();
        Task<double> GetMostBookedRateForRoomAsync(Guid laundryRoomId, DateOnly bookingDate, DateOnly endDate);
        Task<double> GetWeeklyOccupancyRateForRoomAsync(Guid laundryRoomId, DateOnly weekStartDate);
        Task<double> GetTotalOccupancyRateForRoomAsync(Guid laundryRoomId);
    }
}
