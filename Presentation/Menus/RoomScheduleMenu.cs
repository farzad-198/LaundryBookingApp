using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class RoomScheduleMenu
    {
        private readonly IBookingService _bookingService;
        private readonly ILaundryRoomService _laundryRoomService;

        public RoomScheduleMenu(
            IBookingService bookingService,
            ILaundryRoomService laundryRoomService)
        {
            _bookingService = bookingService;
            _laundryRoomService = laundryRoomService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Check a date", ViewRoomScheduleByDate),
                new MenuOption("Check this week", ViewRoomWeeklySchedule),
                new MenuOption("Find free rooms", FindAvailableRooms)
            };

            ConsoleMenu menu = new ConsoleMenu("Check Rooms", options);
            menu.Show();
        }

        public void ViewRoomScheduleByDate()
        {
            Console.Clear();
            Console.WriteLine("=== Room Schedule By Date ===");
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

            DateOnly selectedDate = ReadAnyValidDate("Enter date (yyyy-MM-dd): ");

            List<Booking> bookings = _bookingService
                .GetBookingsForRoomByDateAsync(selectedRoom.Id, selectedDate)
                .Result
                .ToList();

            Console.WriteLine();
            Console.WriteLine($"Room: {selectedRoom.Name}");
            Console.WriteLine($"Location: {selectedRoom.Location}");
            Console.WriteLine($"Date: {selectedDate:yyyy-MM-dd}");
            Console.WriteLine(new string('-', 40));

            ShowSlotStatus(bookings, TimeSlot.Slot09To12);
            ShowSlotStatus(bookings, TimeSlot.Slot12To15);
            ShowSlotStatus(bookings, TimeSlot.Slot15To18);

            Pause();
        }

        public void ViewRoomWeeklySchedule()
        {
            Console.Clear();
            Console.WriteLine("=== Room Weekly Schedule ===");
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

            DateOnly startDate = ReadAnyValidDate("Enter week start date (yyyy-MM-dd): ");
            DateOnly endDate = startDate.AddDays(6);

            List<Booking> weeklyBookings = _bookingService
                .GetBookingsForRoomByWeekAsync(selectedRoom.Id, startDate, endDate)
                .Result
                .ToList();

            Console.WriteLine();
            Console.WriteLine($"Room: {selectedRoom.Name}");
            Console.WriteLine($"Location: {selectedRoom.Location}");
            Console.WriteLine($"Week: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
            Console.WriteLine(new string('=', 50));

            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                // show 7 days
                DateOnly currentDate = startDate.AddDays(dayOffset);
                List<Booking> dayBookings = weeklyBookings
                    .Where(booking => booking.BookingDate == currentDate)
                    .ToList();

                Console.WriteLine();
                Console.WriteLine($"{currentDate:yyyy-MM-dd}");
                Console.WriteLine(new string('-', 30));

                ShowSlotStatus(dayBookings, TimeSlot.Slot09To12);
                ShowSlotStatus(dayBookings, TimeSlot.Slot12To15);
                ShowSlotStatus(dayBookings, TimeSlot.Slot15To18);
            }

            Pause();
        }

        public void FindAvailableRooms()
        {
            Console.Clear();
            Console.WriteLine("=== Find Available Rooms ===");
            Console.WriteLine();

            DateOnly bookingDate = ReadFutureOrTodayDate("Enter booking date (yyyy-MM-dd): ");

            Console.WriteLine();
            Console.WriteLine("Select time slot:");
            Console.WriteLine("1. 09:00-12:00");
            Console.WriteLine("2. 12:00-15:00");
            Console.WriteLine("3. 15:00-18:00");

            int slotNumber = ReadNumberInRange("Enter time slot number: ", 1, 3);
            TimeSlot timeSlot = GetTimeSlotFromNumber(slotNumber);

            List<LaundryRoom> availableRooms = _laundryRoomService
                .GetAvailableRoomsAsync(bookingDate, timeSlot)
                .Result
                .ToList();

            Console.WriteLine();
            Console.WriteLine($"Date: {bookingDate:yyyy-MM-dd}");
            Console.WriteLine($"Time Slot: {GetTimeSlotText(timeSlot)}");
            Console.WriteLine(new string('-', 40));

            if (!availableRooms.Any())
            {
                MessageDisplay.ShowWarning("No available laundry rooms found for this date and time slot.");
                Pause();
                return;
            }

            MessageDisplay.ShowSuccess("Available laundry rooms:");

            foreach (LaundryRoom room in availableRooms)
            {
                Console.WriteLine($"Name: {room.Name}");
                Console.WriteLine($"Location: {room.Location}");
                Console.WriteLine($"Capacity: {room.Capacity}");
                Console.WriteLine($"Dryer: {(room.HasDryer ? "Yes" : "No")}");
                Console.WriteLine($"Iron: {(room.HasIron ? "Yes" : "No")}");
                Console.WriteLine($"Description: {room.Description}");
                Console.WriteLine(new string('-', 40));
            }

            Pause();
        }

        private void ShowSlotStatus(List<Booking> bookings, TimeSlot timeSlot)
        {
            // booking for this slot
            Booking? booking = bookings.FirstOrDefault(b => b.TimeSlot == timeSlot);

            if (booking is null)
            {
                Console.WriteLine($"{GetTimeSlotText(timeSlot)} : Available");
            }
            else
            {
                string personName = booking.Person?.FullName ?? "Unknown";
                Console.WriteLine($"{GetTimeSlotText(timeSlot)} : Booked by {personName}");
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

        private DateOnly ReadAnyValidDate(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (DateOnly.TryParse(input, out DateOnly date))
                {
                    return date;
                }

                MessageDisplay.ShowWarning("Invalid date format. Use yyyy-MM-dd.");
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

        private TimeSlot GetTimeSlotFromNumber(int slotNumber)
        {
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
