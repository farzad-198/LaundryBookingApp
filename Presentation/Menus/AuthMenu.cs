using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Presentation.Helpers;

namespace Presentation.Menus
{
    public class AuthMenu
    {
        private readonly IAuthService _authService;
        private readonly IAdminService _adminService;
        private readonly IBookingService _bookingService;
        private readonly ILaundryRoomService _laundryRoomService;
        private readonly IReportService _reportService;

        public AuthMenu(
            IAuthService authService,
            IAdminService adminService,
            IBookingService bookingService,
            ILaundryRoomService laundryRoomService,
            IReportService reportService)
        {
            _authService = authService;
            _adminService = adminService;
            _bookingService = bookingService;
            _laundryRoomService = laundryRoomService;
            _reportService = reportService;
        }

        public void Show()
        {
            List<MenuOption> options = new List<MenuOption>
            {
                new MenuOption("Login", Login),
                new MenuOption("Register", Register),
                new MenuOption("Exit", Exit)
            };

            ConsoleMenu menu = new ConsoleMenu("Start Menu", options);
            menu.Show();
        }

        private void Login()
        {
            Console.Clear();
            Console.WriteLine("=== Login ===");
            Console.WriteLine();

            string username = ReadRequiredText("Enter username: ");
            string password = ReadRequiredPassword("Enter password: ");

            try
            {
                UserAccount? account = _authService.LoginAsync(username, password).Result;

                if (account is null)
                {
                    MessageDisplay.ShowError("Invalid username or password.");
                    Pause();
                    Show();
                    return;
                }

                MessageDisplay.ShowSuccess($"Welcome, {account.Username}!");
                Pause();

                if (account.Role == UserRole.Admin)
                {
                    AdminMenu adminMenu = new AdminMenu(
                        _adminService,
                        _bookingService,
                        _laundryRoomService,
                        _reportService);

                    adminMenu.Show();
                }
                else
                {
                    ResidentMenu residentMenu = new ResidentMenu(
                        account,
                        _bookingService,
                        _laundryRoomService);

                    residentMenu.Show();
                }

                Show();
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Login failed: {ex.Message}");
                Pause();
                Show();
            }
        }

        private void Register()
        {
            Console.Clear();
            Console.WriteLine("=== Register Resident ===");
            Console.WriteLine();

            string fullName = ReadRequiredText("Enter full name: ");
            string addressOrDepartment = ReadRequiredText("Enter building number / unit / department: ");
            string username = ReadRequiredText("Enter username: ");
            string password = ReadRequiredPassword("Enter password: ");

            try
            {
                UserAccount account = _authService.RegisterResidentAsync(
                    fullName,
                    addressOrDepartment,
                    username,
                    password).Result;

                MessageDisplay.ShowSuccess($"Account created successfully for username: {account.Username}");
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError($"Registration failed: {ex.Message}");
            }

            Pause();
            Show();
        }

        private void Exit()
        {
            Environment.Exit(0);
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

        private string ReadRequiredPassword(string message)
        {
            while (true)
            {
                Console.Write(message);
                string password = string.Empty;

                // hide password text
                while (true)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }

                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (password.Length > 0)
                        {
                            password = password[..^1];
                            // remove one *
                            Console.Write("\b \b");
                        }

                        continue;
                    }

                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        password += keyInfo.KeyChar;
                        // show only *
                        Console.Write("*");
                    }
                }

                if (!string.IsNullOrWhiteSpace(password))
                {
                    return password.Trim();
                }

                MessageDisplay.ShowWarning("This field is required.");
            }
        }

        private void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
