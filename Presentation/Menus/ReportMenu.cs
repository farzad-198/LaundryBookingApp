using Application.Interfaces.Services;
using Domain.Entities;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class ReportMenu
    {
        private readonly IReportService _reportService;
        private readonly ILaundryRoomService _laundryRoomService;

        public ReportMenu(IReportService reportService, ILaundryRoomService laundryRoomService)
        {
            _reportService = reportService;
            _laundryRoomService = laundryRoomService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Most booked laundry room", ShowMostBookedLaundryRoom),
                new MenuOption("Top person", ShowTopPerson),
                new MenuOption("Occupancy rate for selected range", ShowOccupancyRateForRange),
                new MenuOption("Weekly occupancy rate for selected room", ShowWeeklyOccupancyRateForRoom),
                new MenuOption("Total occupancy rate for selected room", ShowTotalOccupancyRateForRoom)
            };

            ConsoleMenu menu = new ConsoleMenu("Reports", options);
            menu.Show();
        }

        private void ShowMostBookedLaundryRoom()
        {
            Console.Clear();
            Console.WriteLine("=== Most Booked Laundry Room ===");
            Console.WriteLine();

            try
            {
                string? result = _reportService.GetMostBookedLaundryRoomAsync().Result;

                if (string.IsNullOrWhiteSpace(result))
                {
                    MessageDisplay.ShowWarning("No data found.");
                }
                else
                {
                    MessageDisplay.ShowSuccess($"Most booked laundry room: {result}");
                }
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Failed to load report: {ex.Message}");
            }

            Pause();
        }

        private void ShowTopPerson()
        {
            Console.Clear();
            Console.WriteLine("=== Top Person ===");
            Console.WriteLine();

            try
            {
                string? result = _reportService.GetTopPersonAsync().Result;

                if (string.IsNullOrWhiteSpace(result))
                {
                    MessageDisplay.ShowWarning("No data found.");
                }
                else
                {
                    MessageDisplay.ShowSuccess($"Top person: {result}");
                }
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Failed to load report: {ex.Message}");
            }

            Pause();
        }

        private void ShowOccupancyRateForRange()
        {
            Console.Clear();
            Console.WriteLine("=== Occupancy Rate For Selected Range ===");
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

            DateOnly startDate = ReadAnyValidDate("Enter start date (yyyy-MM-dd): ");
            DateOnly endDate = ReadAnyValidDate("Enter end date (yyyy-MM-dd): ");

            // end date must be after start
            if (endDate < startDate)
            {
                MessageDisplay.ShowError("End date cannot be earlier than start date.");
                Pause();
                return;
            }

            try
            {
                double result = _reportService
                    .GetMostBookedRateForRoomAsync(rooms[roomIndex - 1].Id, startDate, endDate)
                    .Result;

                Console.WriteLine();
                Console.WriteLine($"Laundry room: {rooms[roomIndex - 1].Name}");
                Console.WriteLine($"Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                MessageDisplay.ShowSuccess($"Occupancy rate: {result:F2}%");
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Failed to load report: {ex.Message}");
            }

            Pause();
        }

        private void ShowWeeklyOccupancyRateForRoom()
        {
            Console.Clear();
            Console.WriteLine("=== Weekly Occupancy Rate For Selected Room ===");
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
            DateOnly weekStartDate = ReadAnyValidDate("Enter week start date (yyyy-MM-dd): ");

            try
            {
                double result = _reportService
                    .GetWeeklyOccupancyRateForRoomAsync(rooms[roomIndex - 1].Id, weekStartDate)
                    .Result;

                Console.WriteLine();
                Console.WriteLine($"Laundry room: {rooms[roomIndex - 1].Name}");
                Console.WriteLine($"Week: {weekStartDate:yyyy-MM-dd} to {weekStartDate.AddDays(6):yyyy-MM-dd}");
                MessageDisplay.ShowSuccess($"Weekly occupancy rate: {result:F2}%");
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Failed to load report: {ex.Message}");
            }

            Pause();
        }

        private void ShowTotalOccupancyRateForRoom()
        {
            Console.Clear();
            Console.WriteLine("=== Total Occupancy Rate For Selected Room ===");
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

            try
            {
                double result = _reportService
                    .GetTotalOccupancyRateForRoomAsync(rooms[roomIndex - 1].Id)
                    .Result;

                Console.WriteLine();
                Console.WriteLine($"Laundry room: {rooms[roomIndex - 1].Name}");
                MessageDisplay.ShowSuccess($"Total occupancy rate: {result:F2}%");
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Failed to load report: {ex.Message}");
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

        private void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }
    }
}
