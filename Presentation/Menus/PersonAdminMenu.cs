using Application.Interfaces.Services;
using Domain.Entities;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class PersonAdminMenu
    {
        private readonly IAdminService _adminService;

        public PersonAdminMenu(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Add new person", AddNewPerson),
                new MenuOption("View all persons", ViewAllPersons),
                new MenuOption("Update person", UpdatePerson),
                new MenuOption("Delete person", DeletePerson)
            };

            ConsoleMenu menu = new ConsoleMenu("Person Management", options);
            menu.Show();
        }

        private void AddNewPerson()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Person ===");
            Console.WriteLine();

            string fullName = ReadRequiredText("Enter full name: ");
            string addressOrDepartment = ReadRequiredText("Enter building number / unit / department: ");

            Person person = new Person
            {
                FullName = fullName,
                AddressOrDepartment = addressOrDepartment
            };

            try
            {
                _adminService.CreatePersonAsync(person).Wait();
                MessageDisplay.ShowSuccess("Person added successfully.");
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Failed to add person: {ex.Message}");
            }

            Pause();
        }

        private void ViewAllPersons()
        {
            Console.Clear();
            Console.WriteLine("=== All Persons ===");
            Console.WriteLine();

            var persons = _adminService.GetAllPersonsAsync().Result.ToList();

            if (!persons.Any())
            {
                MessageDisplay.ShowWarning("No persons found.");
                Pause();
                return;
            }

            foreach (var person in persons)
            {
                Console.WriteLine($"Name: {person.FullName}");
                Console.WriteLine($"Building / Unit / Department: {person.AddressOrDepartment}");
                Console.WriteLine(new string('-', 40));
            }

            Pause();
        }

        private void UpdatePerson()
        {
            Console.Clear();
            Console.WriteLine("=== Update Person ===");
            Console.WriteLine();

            var persons = _adminService.GetAllPersonsAsync().Result.ToList();

            if (!persons.Any())
            {
                MessageDisplay.ShowWarning("No persons found.");
                Pause();
                return;
            }

            for (int i = 0; i < persons.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {persons[i].FullName} - {persons[i].AddressOrDepartment}");
            }

            Console.WriteLine();
            int personIndex = ReadNumberInRange("Enter person number: ", 1, persons.Count);

            Person selectedPerson = persons[personIndex - 1];

            Console.WriteLine();
            Console.WriteLine("Leave empty to keep current value.");
            Console.WriteLine($"Current full name: {selectedPerson.FullName}");
            Console.WriteLine($"Current building / unit / department: {selectedPerson.AddressOrDepartment}");
            Console.WriteLine();

            string fullName = ReadOptionalText("Enter new full name: ", selectedPerson.FullName);
            string addressOrDepartment = ReadOptionalText(
                "Enter new building number / unit / department: ",
                selectedPerson.AddressOrDepartment);

            try
            {
                bool result = _adminService.UpdatePersonAsync(
                    selectedPerson.Id,
                    fullName,
                    addressOrDepartment).Result;

                if (result)
                {
                    MessageDisplay.ShowSuccess("Person updated successfully.");
                }
                else
                {
                    MessageDisplay.ShowError("Person not found.");
                }
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Update failed: {ex.Message}");
            }

            Pause();
        }

        private void DeletePerson()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Person ===");
            Console.WriteLine();

            var persons = _adminService.GetAllPersonsAsync().Result.ToList();

            if (!persons.Any())
            {
                MessageDisplay.ShowWarning("No persons found.");
                Pause();
                return;
            }

            for (int i = 0; i < persons.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {persons[i].FullName} - {persons[i].AddressOrDepartment}");
            }

            Console.WriteLine();
            int personIndex = ReadNumberInRange("Enter person number to delete: ", 1, persons.Count);

            Person selectedPerson = persons[personIndex - 1];

            Console.WriteLine();
            Console.WriteLine("Selected person:");
            Console.WriteLine($"Name: {selectedPerson.FullName}");
            Console.WriteLine($"Building / Unit / Department: {selectedPerson.AddressOrDepartment}");
            Console.WriteLine();

            bool confirm = ReadYesNo("Are you sure you want to delete this person? (yes/no): ");

            if (!confirm)
            {
                MessageDisplay.ShowInfo("Delete operation cancelled.");
                Pause();
                return;
            }

            bool result = _adminService.DeletePersonAsync(selectedPerson.Id).Result;

            if (result)
            {
                MessageDisplay.ShowSuccess("Person deleted successfully.");
            }
            else
            {
                MessageDisplay.ShowError("Person not found.");
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

            // keep old value
            return string.IsNullOrWhiteSpace(input) ? currentValue : input;
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

        private void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }
    }
}
