using Application.Interfaces.Services;
using Domain.Entities;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class ResidentMenu
    {
        private readonly UserAccount _currentUser;
        private readonly IBookingService _bookingService;
        private readonly ILaundryRoomService _laundryRoomService;

        public ResidentMenu(
            UserAccount currentUser,
            IBookingService bookingService,
            ILaundryRoomService laundryRoomService)
        {
            _currentUser = currentUser;
            _bookingService = bookingService;
            _laundryRoomService = laundryRoomService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Manage my bookings", OpenMyBookingMenu),
                new MenuOption("Check room availability", OpenRoomScheduleMenu),
                new MenuOption("Logout", Logout)
            };

            ConsoleMenu menu = new ConsoleMenu($"Resident Panel - {_currentUser.Username}", options);
            menu.Show();
        }

        private void OpenMyBookingMenu()
        {
            MyBookingMenu myBookingMenu = new MyBookingMenu(
                _currentUser,
                _bookingService,
                _laundryRoomService);

            myBookingMenu.Show();
            Show();
        }

        private void OpenRoomScheduleMenu()
        {
            RoomScheduleMenu roomScheduleMenu = new RoomScheduleMenu(
                _bookingService,
                _laundryRoomService);

            roomScheduleMenu.Show();
            Show();
        }

        private void Logout()
        {
        }
    }
}