using Application.Interfaces.Services;
using Infrastructure.Data;
using Infrastructure.Seeding;
using Presentation.Helpers;
using Presentation.Menus;

namespace Presentation.Startup
{
    public class AppRunner
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IAdminService _adminService;
        private readonly IBookingService _bookingService;
        private readonly ILaundryRoomService _laundryRoomService;
        private readonly IReportService _reportService;

        public AppRunner(
            AppDbContext context,
            IAuthService authService,
            IAdminService adminService,
            IBookingService bookingService,
            ILaundryRoomService laundryRoomService,
            IReportService reportService)
        {
            _context = context;
            _authService = authService;
            _adminService = adminService;
            _bookingService = bookingService;
            _laundryRoomService = laundryRoomService;
            _reportService = reportService;
        }

        public async Task RunAsync()
        {
            await LaundryBookingSeeder.SeedAsync(_context);

            WelcomeScreen.Show();

            AuthMenu authMenu = new AuthMenu(
                _authService,
                _adminService,
                _bookingService,
                _laundryRoomService,
                _reportService);

            authMenu.Show();
        }
    }
}
