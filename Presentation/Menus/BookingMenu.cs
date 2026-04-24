using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class BookingMenu
    {
        private readonly IBookingService _bookingService;
        private readonly ILaundryRoomService _laundryRoomService;
        private readonly IAdminService _adminService;

        public BookingMenu(
            IBookingService bookingService,
            ILaundryRoomService laundryRoomService,
            IAdminService adminService)
        {
            _bookingService = bookingService;
            _laundryRoomService = laundryRoomService;
            _adminService = adminService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Create a new booking", CreateBooking),
                new MenuOption("View bookings", ViewBookings),
                new MenuOption("Delete booking", DeleteBooking),
                new MenuOption("Update booking", UpdateBooking)
            };

            ConsoleMenu menu = new ConsoleMenu("Booking Menu", options);
            menu.Show();
        }

        private void CreateBooking()
        {
            Console.Clear();
            Console.WriteLine("=== Create New Booking ===");
            Console.WriteLine();

            List<Person> persons = _adminService.GetAllPersonsAsync().Result.ToList();

            if (!persons.Any())
            {
                MessageDisplay.ShowWarning("No persons found. Please add a person first.");
                Pause();
                return;
            }

            Console.WriteLine("Select person:");
            for (int i = 0; i < persons.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {persons[i].FullName} - {persons[i].AddressOrDepartment}");
            }

            Console.WriteLine();
            int personIndex = ReadNumberInRange("Enter person number: ", 1, persons.Count);
            // selected person
            Person selectedPerson = persons[personIndex - 1];

            List<LaundryRoom> rooms = _laundryRoomService.GetAllLaundryRoomsAsync().Result.ToList();

            if (!rooms.Any())
            {
                MessageDisplay.ShowWarning("No laundry rooms found.");
                Pause();
                return;
            }

            Console.WriteLine();
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
                    selectedPerson.Id,
                    selectedRoom.Id,
                    bookingDate,
                    timeSlot).Result;

                MessageDisplay.ShowSuccess("Booking created successfully.");
                Console.WriteLine($"Name: {selectedPerson.FullName}");
                Console.WriteLine($"Building / Unit / Department: {selectedPerson.AddressOrDepartment}");
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
        private void ViewBookings()
        {
            Console.Clear();
            Console.WriteLine("=== All Bookings ===");
            Console.WriteLine();

            List<Booking> bookings = _bookingService.GetAllBookingsAsync().Result.ToList();

            if (!bookings.Any())
            {
                MessageDisplay.ShowWarning("No bookings found.");
                Pause();
                return;
            }

            foreach (Booking booking in bookings)
            {
                Console.WriteLine($"Name: {booking.Person?.FullName ?? "Unknown"}");
                Console.WriteLine($"Building / Unit / Department: {booking.Person?.AddressOrDepartment ?? "Unknown"}");
                Console.WriteLine($"Room: {booking.LaundryRoom?.Name ?? "Unknown"}");
                Console.WriteLine($"Location: {booking.LaundryRoom?.Location ?? "Unknown"}");
                Console.WriteLine($"Date: {booking.BookingDate:yyyy-MM-dd}");
                Console.WriteLine($"Time Slot: {GetTimeSlotText(booking.TimeSlot)}");
                Console.WriteLine(new string('-', 45));
            }

            Pause();
        }

        private void DeleteBooking()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Booking ===");
            Console.WriteLine();

            List<Booking> bookings = _bookingService.GetAllBookingsAsync().Result.ToList();

            if (!bookings.Any())
            {
                MessageDisplay.ShowWarning("No bookings found.");
                Pause();
                return;
            }

            for (int i = 0; i < bookings.Count; i++)
            {
                Booking booking = bookings[i];
                Console.WriteLine($"{i + 1}. {booking.Person?.FullName ?? "Unknown"} | {booking.LaundryRoom?.Name ?? "Unknown"} | {booking.BookingDate:yyyy-MM-dd} | {GetTimeSlotText(booking.TimeSlot)}");
            }

            Console.WriteLine();
            int index = ReadNumberInRange("Enter booking number to delete: ", 1, bookings.Count);

            Booking selectedBooking = bookings[index - 1];

            Console.WriteLine();
            Console.WriteLine("Selected booking:");
            Console.WriteLine($"Name: {selectedBooking.Person?.FullName ?? "Unknown"}");
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
                MessageDisplay.ShowSuccess("Booking deleted successfully.");
            }
            else
            {
                MessageDisplay.ShowError("Booking not found.");
            }

            Pause();
        }

        private void UpdateBooking()
        {
            Console.Clear();
            Console.WriteLine("=== Update Booking ===");
            Console.WriteLine();

            List<Booking> bookings = _bookingService.GetAllBookingsAsync().Result.ToList();
            List<LaundryRoom> rooms = _laundryRoomService.GetAllLaundryRoomsAsync().Result.ToList();

            if (!bookings.Any())
            {
                MessageDisplay.ShowWarning("No bookings found.");
                Pause();
                return;
            }

            if (!rooms.Any())
            {
                MessageDisplay.ShowWarning("No laundry rooms found.");
                Pause();
                return;
            }

            Console.WriteLine("Select booking to update:");
            for (int i = 0; i < bookings.Count; i++)
            {
                Booking booking = bookings[i];
                Console.WriteLine($"{i + 1}. {booking.Person?.FullName ?? "Unknown"} | {booking.LaundryRoom?.Name ?? "Unknown"} | {booking.BookingDate:yyyy-MM-dd} | {GetTimeSlotText(booking.TimeSlot)}");
            }

            int bookingIndex = ReadNumberInRange("Enter booking number: ", 1, bookings.Count);

            Console.WriteLine();
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
                    bookings[bookingIndex - 1].Id,
                    selectedRoom.Id,
                    bookingDate,
                    timeSlot).Result;

                if (result)
                {
                    MessageDisplay.ShowSuccess("Booking updated successfully.");
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

        private string ReadRequiredText(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }

                MessageDisplay.ShowWarning("This field is required.");
            }
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
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }
    }
}
