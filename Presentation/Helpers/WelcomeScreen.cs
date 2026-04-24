namespace Presentation.Helpers
{
    public static class WelcomeScreen
    {
        public static void Show()
        {
            Console.Clear();

            DateTime now = DateTime.Now;
            string todayDate = now.ToString("yyyy-MM-dd");
            string currentTime = now.ToString("HH:mm:ss");

            string title = "Welcome to Laundry Booking App";
            string dateText = $"Today's Date: {todayDate}";
            string timeText = $"Current Time: {currentTime}";
            string message = "Manage your laundry rooms and bookings easily.";
            string continueText = "Press any key to continue...";

            int centerXTitle = (Console.WindowWidth - title.Length) / 2;
            int centerXDate = (Console.WindowWidth - dateText.Length) / 2;
            int centerXTime = (Console.WindowWidth - timeText.Length) / 2;
            int centerXMessage = (Console.WindowWidth - message.Length) / 2;
            int centerXContinue = (Console.WindowWidth - continueText.Length) / 2;

            int startY = Console.WindowHeight / 2 - 4;

            Console.SetCursorPosition(Math.Max(centerXTitle, 0), Math.Max(startY, 0));
            Console.WriteLine(title);

            Console.SetCursorPosition(Math.Max(centerXDate, 0), Math.Max(startY + 2, 0));
            Console.WriteLine(dateText);

            Console.SetCursorPosition(Math.Max(centerXTime, 0), Math.Max(startY + 3, 0));
            Console.WriteLine(timeText);

            Console.SetCursorPosition(Math.Max(centerXMessage, 0), Math.Max(startY + 5, 0));
            Console.WriteLine(message);

            Console.SetCursorPosition(Math.Max(centerXContinue, 0), Math.Max(startY + 7, 0));
            Console.WriteLine(continueText);

            Console.ReadKey(true);
            Console.Clear();
        }
    }
}