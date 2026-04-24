namespace Presentation.Helpers
{
    public class ConsoleMenu
    {
        private readonly string _title;
        private readonly List<MenuOption> _options;

        public ConsoleMenu(string title, List<MenuOption> options)
        {
            _title = title;
            _options = options;
        }

        public void Show()
        {
            int selectedIndex = 0;
            bool isRunning = true;

            while (isRunning)
            {
                DrawMenu(selectedIndex);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex--;
                        if (selectedIndex < 0)
                        {
                            // back to last
                            selectedIndex = _options.Count - 1;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex++;
                        if (selectedIndex >= _options.Count)
                        {
                            // back to first
                            selectedIndex = 0;
                        }
                        break;

                    case ConsoleKey.Enter:
                        Console.Clear();
                        _options[selectedIndex].Action.Invoke();
                        isRunning = false;
                        break;

                    case ConsoleKey.Backspace:
                        isRunning = false;
                        break;
                }
            }
        }

        private void DrawMenu(int selectedIndex)
        {
            Console.Clear();

            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            int totalLines = _options.Count + 2;
            int startTop = (windowHeight / 2) - (totalLines / 2);

            WriteCentered(_title, startTop - 2);

            for (int i = 0; i < _options.Count; i++)
            {
                string prefix = i == selectedIndex ? "> " : "  ";
                string text = prefix + _options[i].Text;
                WriteCentered(text, startTop + i);
            }

            WriteCentered("Use Up/Down arrows, Enter to select, Backspace to go back", startTop + _options.Count + 2);
        }

        private void WriteCentered(string text, int top)
        {
            // no minus position
            int left = Math.Max((Console.WindowWidth - text.Length) / 2, 0);

            Console.SetCursorPosition(left, Math.Max(top, 0));
            Console.Write(text);
        }
    }
}
