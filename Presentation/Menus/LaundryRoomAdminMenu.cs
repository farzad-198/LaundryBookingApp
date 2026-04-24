using Application.Interfaces.Services;
using Domain.Entities;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class LaundryRoomAdminMenu
    {
        private readonly IAdminService _adminService;

        public LaundryRoomAdminMenu(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Add new laundry room", AddNewLaundryRoom),
                new MenuOption("View all laundry rooms", ViewAllLaundryRooms),
                new MenuOption("Update laundry room", UpdateLaundryRoom),
                new MenuOption("Delete laundry room", DeleteLaundryRoom)
            };

            ConsoleMenu menu = new ConsoleMenu("Laundry Room Management", options);
            menu.Show();
        }

        private void AddNewLaundryRoom()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Laundry Room ===");
            Console.WriteLine();

            string name = ReadRequiredText("Enter room name: ");
            string location = ReadRequiredText("Enter location: ");
            int capacity = ReadPositiveNumber("Enter capacity: ");
            string description = ReadOptionalFreeText("Enter description (optional): ");

            bool hasDryer = ReadYesNo("Has dryer? (yes/no): ");
            bool hasIron = ReadYesNo("Has iron? (yes/no): ");

            LaundryRoom room = new LaundryRoom
            {
                Name = name,
                Location = location,
                Capacity = capacity,
                HasDryer = hasDryer,
                HasIron = hasIron,
                Description = description
            };

            try
            {
                _adminService.CreateLaundryRoomAsync(room).Wait();
                MessageDisplay.ShowSuccess("Laundry room added successfully.");
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Failed to add laundry room: {ex.Message}");
            }

            Pause();
        }

        private void ViewAllLaundryRooms()
        {
            Console.Clear();
            Console.WriteLine("=== All Laundry Rooms ===");
            Console.WriteLine();

            var rooms = _adminService.GetAllLaundryRoomsAsync().Result.ToList();

            if (!rooms.Any())
            {
                MessageDisplay.ShowWarning("No laundry rooms found.");
                Pause();
                return;
            }

            foreach (var room in rooms)
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

        private void UpdateLaundryRoom()
        {
            Console.Clear();
            Console.WriteLine("=== Update Laundry Room ===");
            Console.WriteLine();

            var rooms = _adminService.GetAllLaundryRoomsAsync().Result.ToList();

            if (!rooms.Any())
            {
                MessageDisplay.ShowWarning("No laundry rooms found.");
                Pause();
                return;
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rooms[i].Name} - {rooms[i].Location}");
            }

            Console.WriteLine();
            int roomIndex = ReadNumberInRange("Enter room number: ", 1, rooms.Count);

            LaundryRoom selectedRoom = rooms[roomIndex - 1];

            Console.WriteLine();
            Console.WriteLine("Leave text empty to keep current value.");
            Console.WriteLine($"Current name: {selectedRoom.Name}");
            Console.WriteLine($"Current location: {selectedRoom.Location}");
            Console.WriteLine($"Current capacity: {selectedRoom.Capacity}");
            Console.WriteLine($"Current dryer: {(selectedRoom.HasDryer ? "Yes" : "No")}");
            Console.WriteLine($"Current iron: {(selectedRoom.HasIron ? "Yes" : "No")}");
            Console.WriteLine($"Current description: {selectedRoom.Description}");
            Console.WriteLine();

            string name = ReadOptionalText("Enter new room name: ", selectedRoom.Name);
            string location = ReadOptionalText("Enter new location: ", selectedRoom.Location);
            int capacity = ReadOptionalPositiveNumber("Enter new capacity: ", selectedRoom.Capacity);
            string description = ReadOptionalText("Enter new description: ", selectedRoom.Description ?? string.Empty);

            bool hasDryer = ReadYesNoWithDefault(
                $"Has dryer? (yes/no) [current: {(selectedRoom.HasDryer ? "yes" : "no")}]: ",
                selectedRoom.HasDryer);

            bool hasIron = ReadYesNoWithDefault(
                $"Has iron? (yes/no) [current: {(selectedRoom.HasIron ? "yes" : "no")}]: ",
                selectedRoom.HasIron);

            try
            {
                bool result = _adminService.UpdateLaundryRoomAsync(
                    selectedRoom.Id,
                    name,
                    location,
                    capacity,
                    hasDryer,
                    hasIron,
                    description).Result;

                if (result)
                {
                    MessageDisplay.ShowSuccess("Laundry room updated successfully.");
                }
                else
                {
                    MessageDisplay.ShowError("Laundry room not found.");
                }
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Update failed: {ex.Message}");
            }

            Pause();
        }

        private void DeleteLaundryRoom()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Laundry Room ===");
            Console.WriteLine();

            var rooms = _adminService.GetAllLaundryRoomsAsync().Result.ToList();

            if (!rooms.Any())
            {
                MessageDisplay.ShowWarning("No laundry rooms found.");
                Pause();
                return;
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rooms[i].Name} - {rooms[i].Location}");
            }

            Console.WriteLine();
            int roomIndex = ReadNumberInRange("Enter room number to delete: ", 1, rooms.Count);

            LaundryRoom selectedRoom = rooms[roomIndex - 1];

            Console.WriteLine();
            Console.WriteLine("Selected laundry room:");
            Console.WriteLine($"Name: {selectedRoom.Name}");
            Console.WriteLine($"Location: {selectedRoom.Location}");
            Console.WriteLine($"Capacity: {selectedRoom.Capacity}");
            Console.WriteLine($"Dryer: {(selectedRoom.HasDryer ? "Yes" : "No")}");
            Console.WriteLine($"Iron: {(selectedRoom.HasIron ? "Yes" : "No")}");
            Console.WriteLine();

            bool confirm = ReadYesNo("Are you sure you want to delete this laundry room? (yes/no): ");

            if (!confirm)
            {
                MessageDisplay.ShowInfo("Delete operation cancelled.");
                Pause();
                return;
            }

            bool result = _adminService.DeleteLaundryRoomAsync(selectedRoom.Id).Result;

            if (result)
            {
                MessageDisplay.ShowSuccess("Laundry room deleted successfully.");
            }
            else
            {
                MessageDisplay.ShowError("Laundry room not found.");
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

        private string ReadOptionalText(string message, string currentValue)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();

            return string.IsNullOrWhiteSpace(input) ? currentValue : input;
        }

        private string ReadOptionalFreeText(string message)
        {
            Console.Write(message);
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        private int ReadPositiveNumber(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int number) && number > 0)
                {
                    return number;
                }

                MessageDisplay.ShowWarning("Invalid input. Enter a number greater than 0.");
            }
        }

        private int ReadOptionalPositiveNumber(string message, int currentValue)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine()?.Trim();

                // keep old value
                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                if (int.TryParse(input, out int number) && number > 0)
                {
                    return number;
                }

                MessageDisplay.ShowWarning("Invalid input. Enter a number greater than 0, or leave empty.");
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

        private bool ReadYesNoWithDefault(string message, bool currentValue)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine()?.Trim().ToLower();

                // keep old value
                if (string.IsNullOrWhiteSpace(input))
                {
                    return currentValue;
                }

                if (input == "yes")
                {
                    return true;
                }

                if (input == "no")
                {
                    return false;
                }

                MessageDisplay.ShowWarning("Invalid input. Please type only 'yes' or 'no', or leave empty.");
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
