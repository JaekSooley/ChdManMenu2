using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleUI
{
    /// <summary>
    /// Kinda just another way of doing menus instead of manually adding UI.Options.
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// Text displayed in the menu header.
        /// </summary>
        public string name = "Menu";

        /// <summary>
        /// Description text displayed below the menu's header.
        /// </summary>
        public string description = "";

        /// <summary>
        /// Text displayed above menu items
        /// </summary>
        public string prompt = "Select an option";

        /// <summary>
        /// Root menus do not break their loop when an item is selected.
        /// </summary>
        public bool isRootMenu = false;

        /// <summary>
        /// Width of the menu, in characters.
        /// </summary>
        public int width = 56;

        private ConsoleColor foreground = Console.ForegroundColor;
        private ConsoleColor background = Console.BackgroundColor;
        private List<Item> items = new List<Item>();
        private string logPath = AppDomain.CurrentDomain.BaseDirectory + "log.txt";

        /// <summary>
        /// Handles display and selection of menu items.
        /// </summary>
        public void Make()
        {
            int index = 0;

            while (true)
            {
                // Draw header
                DrawHeader();

                // Draw standard text
                Console.WriteLine();

                if (!string.IsNullOrEmpty(description))
                {
                    Console.WriteLine();
                    Console.WriteLine(description);
                    Console.WriteLine();
                }

                if (!string.IsNullOrEmpty(prompt))
                {
                    Console.WriteLine();
                    Console.WriteLine(prompt);
                    for (int i = 0; i <= width; i++) Console.Write("-");
                }

                Console.WriteLine();

                // Draw menu options
                for (int i = 0; i < items.Count; i++)
                {
                    if (i == index)
                    {
                        DrawItemHighlight(items[i]);
                    }
                    else
                    {
                        DrawItem(items[i]);
                    }
                }

                // Bottom of menu line
                for (int i = 0; i <= width; i++) Console.Write("-");
                Console.Write("\n");

                // Draw item description
                if (!string.IsNullOrEmpty(items[index].description))
                {

                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    Console.WriteLine("Description:");
                    Console.WriteLine($"{items[index].description}");
                    Console.WriteLine();

                    Console.ForegroundColor = foreground;
                }

                // Get input
                var key = Console.ReadKey();

                // Navigation
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (index > 0) index -= 1;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (index < items.Count - 1) index += 1;
                }

                if (key.Key == ConsoleKey.RightArrow) index = items.Count - 1;
                if (key.Key == ConsoleKey.LeftArrow) index = 0;

                // Confirm
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.Clear();

                    items[index].method();

                    if (isRootMenu) continue;
                    else break;
                }
            }
        }

        /// <summary>
        /// Draws the menu header
        /// </summary>
        private void DrawHeader()
        {
            Console.Clear();

            foreground = Console.ForegroundColor;
            background = Console.BackgroundColor;

            Console.BackgroundColor = foreground;
            Console.ForegroundColor = background;

            StringBuilder sbSpacer = new();
            for (int i = 0; i <= width; i++) sbSpacer.Append(' ');

            StringBuilder sbText = new();
            for (int i = sbText.Length; i <= (width / 2) - (name.Length / 2); i++)
            {
                sbText.Append(" ");
            }
            sbText.Append(name);
            while (sbText.Length <= width) sbText.Append(" ");

            Console.WriteLine($"{sbSpacer}\n{sbText}\n{sbSpacer}");

            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }

        /// <summary>
        /// Draws a menu option
        /// </summary>
        /// <param name="item"></param>
        private void DrawItem(Item item)
        {
            string label = $"  {item.text}";
            Console.WriteLine(label);
        }

        /// <summary>
        /// Draws a highlighted menu option.
        /// </summary>
        /// <param name="item"></param>
        private void DrawItemHighlight(Item item)
        {
            string spacer = "    ";
            string label = spacer + item.text;
            
            Console.BackgroundColor = foreground;
            Console.ForegroundColor = background;

            StringBuilder sb = new();
            sb.Append(label);
            while (sb.Length <= width) sb.Append(" ");
            sb.Append("\n");
            Console.Write(sb.ToString());

            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }

        /// <summary>
        /// Empty method used for menu items that don't do anything.
        /// </summary>
        public void Null() { }

        /// <summary>
        /// ConsoleUI menu used mostly for log file management.
        /// </summary>
        private void InternalMenu()
        {
            Menu menu = new Menu();
            menu.name = "ConsoleUI";
            menu.description = "Manage ConsoleUI";

            if (File.Exists(logPath))
            {
                menu.Add("Open log file", OpenLogFile);
                menu.Add("Delete log file", DeleteLogFile);
            }
            menu.AddReturnButton();

            menu.Make();
        }

        private void OpenLogFile()
        {
            if (File.Exists(logPath)) Process.Start("notepad.exe", logPath);
            else
            {
                Console.WriteLine("\"log.txt\" does not exist!");
                UI.Pause();
            }
        }

        private void DeleteLogFile()
        {
            File.Delete(logPath);
            Console.WriteLine("Deleted \"log.txt\"");
            UI.Pause();
        }

        /// <summary>
        /// Adds a new item to the menu
        /// </summary>
        /// <param name="text"></param>
        /// <param name="method">
        /// Method to be executed when menu item is selected.
        /// </param>
        public void Add(string text, ItemMethod method, string description = "")
        {
            Item item = new();

            item.text = text;
            item.method = method;
            item.description = description;

            items.Add(item);
        }

        /// <summary>
        ///  Adds a nifty lil' back button to a menu.
        /// </summary>
        /// <param name="text">
        /// Overrides default text ("Back")
        /// </param>
        public void AddReturnButton(string text = "Back")
        {
            Item item = new();

            item.text = text;
            item.method = Null;
            item.description = "";

            items.Add(item);
        }

        public delegate void ItemMethod();

        internal class Item()
        {
            public string text = string.Empty;
            public ItemMethod method;
            public string description = string.Empty;
        }
    }
}
