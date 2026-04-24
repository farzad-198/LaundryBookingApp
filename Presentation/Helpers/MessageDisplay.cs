namespace Presentation.Helpers
{
    public static class MessageDisplay
    {
        public static void ShowSuccess(string message)
        {
            Console.WriteLine();
            Console.WriteLine($"[SUCCESS] {message}");
        }

        public static void ShowError(string message)
        {
            Console.WriteLine();
            Console.WriteLine($"[ERROR] {message}");
        }

        public static void ShowWarning(string message)
        {
            Console.WriteLine();
            Console.WriteLine($"[WARNING] {message}");
        }

        public static void ShowInfo(string message)
        {
            Console.WriteLine();
            Console.WriteLine($"[INFO] {message}");
        }
    }
}