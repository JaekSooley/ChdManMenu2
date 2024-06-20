namespace ConsoleUI
{
    /// <summary>
    /// Controls the display of ConsoleUI text.
    /// </summary>
    public class UI
    {
        /// <summary>
        /// Menu/screen header with centred text.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="clearPreviousScreen"></param>
        /// <param name="paddingCharacter"></param>
        public static void Header(string header, bool clearPreviousScreen = true, ConsoleColor color = ConsoleColor.White, string paddingCharacter = "=")
        {
            if (clearPreviousScreen) Console.Clear();

            int headerWidth = 56;
            int textWidth = header.Length + 2;
            int paddingWidth = (headerWidth - textWidth) / 2;

            string paddingString = "";
            for (int i = 0; i < paddingWidth; i++) paddingString += paddingCharacter;

            string displayText = paddingString;
            displayText += $" {header} ";
            displayText += paddingString;

            Console.ForegroundColor = color;
            Console.WriteLine("");
            Console.WriteLine(displayText);
            Console.WriteLine("");
        }

        /// <summary>
        /// Menu options that use keywords (e.g. Input OPEN to select the [OPEN] option)
        /// </summary>
        /// <param name="option"></param>
        /// <param name="description"></param>
        /// <param name="current"></param>
        public static void Option(string option, string description = "", string current = "")
        {
            int descriptionOffset = 24;

            string text = $"\t{option}";
            string whitespace = "";

            if (text.Length <= descriptionOffset)
            {
                for (int i = 0; i <= descriptionOffset - text.Length; i++)
                {
                    whitespace += " ";
                }
            }

            if (current != "") text += $" ({current})";
            if (description != "") text += $"{whitespace} - {description}";
            Console.WriteLine(text);
        }

        /// <summary>
        /// Menu options that use index numbers (e.g. Input [1] to select first option)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="description"></param>
        /// <param name="current"></param>
        public static void IndexOption(int index, string description = "", string current = "")
        {
            int descriptionOffset = 5;

            string text = $"  [{index}]";
            string whitespace = "";

            if (text.Length <= descriptionOffset)
            {
                for (int i = 0; i <= descriptionOffset - text.Length; i++)
                {
                    whitespace += " ";
                }
            }

            if (current != "") text += $" ({current})";
            if (description != "") text += $"{whitespace} {description}";
            Console.WriteLine(text);
        }

        /// <summary>
        /// Pause console and await keypress.
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine("");
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Error screen with option to display an error message.
        /// The error message is written to log.txt by default.
        /// Clears previous console lines.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="writeToLog"></param>
        public static void Error(string description, bool writeToLog = true)
        {
            Header("Error", true, ConsoleColor.Red, "/");
            Console.WriteLine(description);

            if (writeToLog)
            {
                string fname = "log.txt";
                string path = AppDomain.CurrentDomain.BaseDirectory + fname;
                string time = DateTime.Now.ToString();
                string text = $"{time} | ERROR: {description}";

                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                }
            }

            Pause();

            Console.ResetColor();
        }


        /// <summary>
        /// Friendly little warning screen. Does not clear console.
        /// Includes option to write warning to log.txt (disabled by default).
        /// </summary>
        /// <param name="description"></param>
        /// <param name="writeToLog"></param>
        public static void Warning(string description, bool writeToLog = false)
        {
            Header("Warning", false, ConsoleColor.DarkYellow, "/");
            Console.WriteLine(description);

            if (writeToLog)
            {
                string fname = "log.txt";
                string path = AppDomain.CurrentDomain.BaseDirectory + fname;
                string time = DateTime.Now.ToString();
                string text = $"{time} | WARNING: {description}";

                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                }
            }

            Pause();

            Console.ResetColor();
        }


        /// <summary>
        /// Literally just Console.WriteLine() lmao
        /// </summary>
        /// <param name="text"></param>
        public static void Write(string text = "", ConsoleColor? color = null)
        {
            if (color != null)
            {
                ConsoleColor previousColor = Console.ForegroundColor;

                Console.ForegroundColor = (ConsoleColor)color;

                Console.WriteLine(text);

                Console.ForegroundColor = previousColor;
            }
            else
            {
                Console.WriteLine(text);
            }
        }


        // Used by the Input class, don't call directly.
        public static void Input(string type, string? defaultValue = null)
        {
            Console.WriteLine("");
            Console.WriteLine($"Input ({type}):");

            Console.Write($"-> ");
            if (defaultValue != null) Console.Write($"[{defaultValue}] ");
        }

        public static void InvertConsoleColors()
        {
            ConsoleColor foreground = Console.ForegroundColor;
            ConsoleColor background = Console.BackgroundColor;

            Console.ForegroundColor = background;
            Console.BackgroundColor = foreground;
        }
    }
}
