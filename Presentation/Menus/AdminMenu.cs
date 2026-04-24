using Application.Interfaces.Services;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class AdminMenu
    {
        private readonly IAdminService _adminService;
        private readonly IBookingService _bookingService;
        private readonly ILaundryRoomService _laundryRoomService;
        private readonly IReportService _reportService;

        public AdminMenu(
            IAdminService adminService,
            IBookingService bookingService,
            ILaundryRoomService laundryRoomService,
            IReportService reportService)
        {
            _adminService = adminService;
            _bookingService = bookingService;
            _laundryRoomService = laundryRoomService;
            _reportService = reportService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Person management", OpenPersonManagement),
                new MenuOption("Laundry room management", OpenLaundryRoomManagement),
                new MenuOption("Booking management", OpenBookingManagement),
                new MenuOption("Reports", OpenReports),
                new MenuOption("Logout", Logout)
            };

            ConsoleMenu menu = new ConsoleMenu("Admin Panel", options);
            menu.Show();
        }

        private void OpenPersonManagement()
        {
            PersonAdminMenu personAdminMenu = new PersonAdminMenu(_adminService);
            personAdminMenu.Show();
            Show();
        }

        private void OpenLaundryRoomManagement()
        {
            LaundryRoomAdminMenu laundryRoomAdminMenu = new LaundryRoomAdminMenu(_adminService);
            laundryRoomAdminMenu.Show();
            Show();
        }

        private void OpenBookingManagement()
        {
            BookingMenu bookingMenu = new BookingMenu(
                _bookingService,
                _laundryRoomService,
                _adminService);

            bookingMenu.Show();
            Show();
        }

        private void OpenReports()
        {
            ReportMenu reportMenu = new ReportMenu(
                _reportService,
                _laundryRoomService);

            reportMenu.Show();
            Show();
        }

        private void Logout()
        {
        }
    }
}