using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class MyBookingMenu
    {
        private readonly UserAccount _currentUser;
        private readonly IBookingService _bookingService;
        private readonly ILaundryRoomService _laundryRoomService;

        public MyBookingMenu(
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
                new MenuOption("Create booking", CreateMyBooking),
                new MenuOption("View my bookings", ViewMyBookings),
                new MenuOption("Update my booking", UpdateMyBooking),
                new MenuOption("Delete my booking", DeleteMyBooking)
            };

            ConsoleMenu menu = new ConsoleMenu($"Manage My Bookings - {_currentUser.Username}", options);
            menu.Show();
        }

        private void CreateMyBooking()
        {
            Console.Clear();
            Console.WriteLine("=== Create My Booking ===");
            Console.WriteLine();

            List<LaundryRoom> rooms = _laundryRoomService.GetAllLaundryRoomsAsync().Result.ToList();

            if (!rooms.Any())
            {
                MessageDisplay.ShowWarning("No laundry rooms found.");
                Pause();
                return;
            }

            Console.WriteLine("Select laundry room:");
            for (int i = 0; i < rooms.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rooms[i].Name} - {rooms[i].Location}");
            }

            int roomIndex = ReadNumberInRange("Enter room number: ", 1, rooms.Count);
            LaundryRoom selectedRoom = rooms[roomIndex - 1];

            DateOnly bookingDate = ReadFutureOrTodayDate("Enter booking date (yyyy-MM-dd): ");

            Console.WriteLine();
            Console.WriteLine("Select time slot:");
            Console.WriteLine("1. 09:00-12:00");
            Console.WriteLine("2. 12:00-15:00");
            Console.WriteLine("3. 15:00-18:00");

            int slotNumber = ReadNumberInRange("Enter time slot number: ", 1, 3);
            TimeSlot timeSlot = GetTimeSlotFromNumber(slotNumber);

            try
            {
                Booking booking = _bookingService.CreateBookingAsync(
                    _currentUser.PersonId,
                    selectedRoom.Id,
                    bookingDate,
                    timeSlot).Result;

                MessageDisplay.ShowSuccess("Booking created successfully.");
                Console.WriteLine($"Room: {selectedRoom.Name}");
                Console.WriteLine($"Location: {selectedRoom.Location}");
                Console.WriteLine($"Date: {booking.BookingDate:yyyy-MM-dd}");
                Console.WriteLine($"Time Slot: {GetTimeSlotText(booking.TimeSlot)}");
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Booking failed: {ex.Message}");
            }

            Pause();
        }

        private void ViewMyBookings()
        {
            Console.Clear();
            Console.WriteLine("=== My Bookings ===");
            Console.WriteLine();

            List<Booking> bookings = _bookingService
                .GetBookingsForPersonAsync(_currentUser.PersonId)
                .Result
                .ToList();

            if (!bookings.Any())
            {
                MessageDisplay.ShowWarning("You have no bookings.");
                Pause();
                return;
            }

            foreach (Booking booking in bookings)
            {
                Console.WriteLine($"Room: {booking.LaundryRoom?.Name ?? "Unknown"}");
                Console.WriteLine($"Location: {booking.LaundryRoom?.Location ?? "Unknown"}");
                Console.WriteLine($"Date: {booking.BookingDate:yyyy-MM-dd}");
                Console.WriteLine($"Time Slot: {GetTimeSlotText(booking.TimeSlot)}");
                Console.WriteLine(new string('-', 40));
            }

            Pause();
        }

        private void UpdateMyBooking()
        {
            Console.Clear();
            Console.WriteLine("=== Update My Booking ===");
            Console.WriteLine();

            List<Booking> myBookings = _bookingService
                .GetBookingsForPersonAsync(_currentUser.PersonId)
                .Result
                .ToList();

            List<LaundryRoom> rooms = _laundryRoomService.GetAllLaundryRoomsAsync().Result.ToList();

            if (!myBookings.Any())
            {
                MessageDisplay.ShowWarning("You have no bookings.");
                Pause();
                return;
            }

            if (!rooms.Any())
            {
                MessageDisplay.ShowWarning("No laundry rooms found.");
                Pause();
                return;
            }

            Console.WriteLine("Select your booking:");
            for (int i = 0; i < myBookings.Count; i++)
            {
                Booking booking = myBookings[i];
                Console.WriteLine($"{i + 1}. {booking.LaundryRoom?.Name ?? "Unknown"} | {booking.BookingDate:yyyy-MM-dd} | {GetTimeSlotText(booking.TimeSlot)}");
            }

            int bookingIndex = ReadNumberInRange("Enter booking number: ", 1, myBookings.Count);

            Console.WriteLine();
            // choose new room
            Console.WriteLine("Select new laundry room:");
            for (int i = 0; i < rooms.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rooms[i].Name} - {rooms[i].Location}");
            }

            int roomIndex = ReadNumberInRange("Enter room number: ", 1, rooms.Count);
            LaundryRoom selectedRoom = rooms[roomIndex - 1];

            DateOnly bookingDate = ReadFutureOrTodayDate("Enter new booking date (yyyy-MM-dd): ");

            Console.WriteLine();
            Console.WriteLine("Select new time slot:");
            Console.WriteLine("1. 09:00-12:00");
            Console.WriteLine("2. 12:00-15:00");
            Console.WriteLine("3. 15:00-18:00");

            int slotNumber = ReadNumberInRange("Enter time slot number: ", 1, 3);
            TimeSlot timeSlot = GetTimeSlotFromNumber(slotNumber);

            try
            {
                bool result = _bookingService.UpdateBookingAsync(
                    myBookings[bookingIndex - 1].Id,
                    selectedRoom.Id,
                    bookingDate,
                    timeSlot).Result;

                if (result)
                {
                    MessageDisplay.ShowSuccess("Your booking was updated successfully.");
                }
                else
                {
                    MessageDisplay.ShowError("Booking not found.");
                }
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Update failed: {ex.Message}");
            }

            Pause();
        }

        private void DeleteMyBooking()
        {
            Console.Clear();
            Console.WriteLine("=== Delete My Booking ===");
            Console.WriteLine();

            List<Booking> myBookings = _bookingService
                .GetBookingsForPersonAsync(_currentUser.PersonId)
                .Result
                .ToList();

            if (!myBookings.Any())
            {
                MessageDisplay.ShowWarning("You have no bookings.");
                Pause();
                return;
            }

            Console.WriteLine("Select your booking to delete:");
            for (int i = 0; i < myBookings.Count; i++)
            {
                Booking booking = myBookings[i];
                Console.WriteLine($"{i + 1}. {booking.LaundryRoom?.Name ?? "Unknown"} | {booking.BookingDate:yyyy-MM-dd} | {GetTimeSlotText(booking.TimeSlot)}");
            }

            int bookingIndex = ReadNumberInRange("Enter booking number: ", 1, myBookings.Count);

            Booking selectedBooking = myBookings[bookingIndex - 1];

            Console.WriteLine();
            Console.WriteLine("Selected booking:");
            Console.WriteLine($"Room: {selectedBooking.LaundryRoom?.Name ?? "Unknown"}");
            Console.WriteLine($"Date: {selectedBooking.BookingDate:yyyy-MM-dd}");
            Console.WriteLine($"Time Slot: {GetTimeSlotText(selectedBooking.TimeSlot)}");
            Console.WriteLine();

            bool confirm = ReadYesNo("Are you sure you want to delete this booking? (yes/no): ");

            if (!confirm)
            {
                MessageDisplay.ShowInfo("Delete operation cancelled.");
                Pause();
                return;
            }

            bool result = _bookingService.DeleteBookingAsync(selectedBooking.Id).Result;

            if (result)
            {
                MessageDisplay.ShowSuccess("Your booking was deleted successfully.");
            }
            else
            {
                MessageDisplay.ShowError("Booking not found.");
            }

            Pause();
        }

        private int ReadNumberInRange(string message, int min, int max)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int number) && number >= min && number <= max)
                {
                    return number;
                }

                MessageDisplay.ShowWarning($"Invalid input. Enter a number between {min} and {max}.");
            }
        }

        private DateOnly ReadFutureOrTodayDate(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (!DateOnly.TryParse(input, out DateOnly date))
                {
                    MessageDisplay.ShowWarning("Invalid date format. Use yyyy-MM-dd.");
                    continue;
                }

                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                // no past date
                if (date < today)
                {
                    MessageDisplay.ShowWarning("Booking date cannot be in the past.");
                    continue;
                }

                return date;
            }
        }

        private bool ReadYesNo(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine()?.Trim().ToLower();

                if (input == "yes")
                {
                    return true;
                }

                if (input == "no")
                {
                    return false;
                }

                MessageDisplay.ShowWarning("Invalid input. Please type only 'yes' or 'no'.");
            }
        }

        private TimeSlot GetTimeSlotFromNumber(int slotNumber)
        {
            // menu number to slot
            return slotNumber switch
            {
                1 => TimeSlot.Slot09To12,
                2 => TimeSlot.Slot12To15,
                3 => TimeSlot.Slot15To18,
                _ => throw new InvalidOperationException("Invalid time slot number.")
            };
        }

        private string GetTimeSlotText(TimeSlot timeSlot)
        {
            return timeSlot switch
            {
                TimeSlot.Slot09To12 => "09:00-12:00",
                TimeSlot.Slot12To15 => "12:00-15:00",
                TimeSlot.Slot15To18 => "15:00-18:00",
                _ => "Unknown"
            };
        }

        private void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
