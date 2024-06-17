using System.Diagnostics;
using System.Text;


namespace ConsoleUI
{
    /// <summary>
    /// Methods used for standardising the display of """UI""" elements
    /// </summary>
    public class UI
    {
        /// <summary>
        /// Menu/screen header with centred text.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="clearPreviousScreen"></param>
        /// <param name="paddingCharacter"></param>
        public static void Header(string header, bool clearPreviousScreen = true, string paddingCharacter = "=")
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
            Header("Error", true, "/");
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
        }

        /// <summary>
        /// Friendly little warning screen. Does not clear console.
        /// Includes option to write warning to log.txt (disabled by default).
        /// </summary>
        /// <param name="description"></param>
        /// <param name="writeToLog"></param>
        public static void Warning(string description, bool writeToLog = false)
        {
            Header("Warning", false, "/");
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
        }

        /// <summary>
        /// Literally just Console.WriteLine() lmao
        /// </summary>
        /// <param name="text"></param>
        public static void Write(string text = "")
        {
            Console.WriteLine(text);
        }


        // Used by the Input class, don't call directly.
        public static void Input(string type, string? defaultValue = null)
        {
            Console.WriteLine("");
            Console.WriteLine($"Input ({type}):");
            Console.Write($"-> ");
            if (defaultValue != null) Console.Write($"[{defaultValue}] ");
        }
    }


    /// <summary>
    /// Methods used for getting inputs from the user.
    /// </summary>
    public class Input
    {
        public static bool? GetBoolean(bool? defaultValue = null)
        {
            string type = "bool";
            bool? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
                output = null;
            }
            
            if (bool.TryParse(input, out bool val)) output = val;

            return output;
        }

        public static int? GetInteger(int? defaultValue = null)
        {
            string type = "int";
            int? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string? input = Console.ReadLine();

            if (int.TryParse(input, out int val)) output = val;

            return output;
        }

        public static float? GetFloat(float? defaultValue = null)
        {
            string type = "float";
            float? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
                output = null;
            }

            if (float.TryParse(input, out float val)) output = val;

            return output;
        }

        public static double? GetDouble(double? defaultValue = null)
        {
            string type = "double";
            double? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
                output = null;
            }

            if (double.TryParse(input, out double val)) output = val;

            return output;
        }


        public static decimal? GetDecimal(decimal? defaultValue = null)
        {
            string type = "decimal";
            decimal? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
                output = null;
            }

            if (decimal.TryParse(input, out decimal val)) output = val;

            return output;
        }

        public static string? GetString(string? defaultValue = null)
        {
            string type = "string";
            string? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue);
            else UI.Input(type);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
                output = null;
            }

            if (input != "") output = input;

            return output;
        }

        /// <summary>
        /// Returns a single file if a valid path is entered. Otherwise, returns null.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string? GetFile(string? defaultValue = null)
        {
            string type = "file path";
            string? output = defaultValue;

            if (!File.Exists(output)) output = null;

            if (defaultValue != null) UI.Input(type, defaultValue);
            else UI.Input(type);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
                output = null;
            }

            if (input != null) input = input.Replace("\"", "");

            if (File.Exists(input)) output = input;

            return output;
        }

        /// <summary>
        /// Returns a list of files that exist based on an input string.
        /// Can be filtered by an extension.
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string? ext = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            string type = "directory/file path(s)";
            List<string> validFiles = [];
            List<string> output = [];

            UI.Input(type);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
            }

            if (input != null)
            {
                List<string> items = [];

                // I guess we're reading inputs per-character smh
                bool inQuotes = false;
                StringBuilder itemSb = new StringBuilder();

                foreach (char c in input)
                {
                    if (c == '"')
                    {
                        inQuotes = !inQuotes;
                        items.Add(itemSb.ToString());
                        itemSb.Clear();
                        continue;
                    }
                    else
                    {
                        if (char.IsWhiteSpace(c) && !inQuotes)
                        {
                            items.Add(itemSb.ToString());
                            itemSb.Clear();
                            continue;
                        }
                        else
                        {
                            itemSb.Append(c);
                        }
                    }
                }

                // Add last item to list
                if (itemSb.Length > 0)
                {
                    items.Add(itemSb.ToString());
                    itemSb.Clear();
                }

                foreach (string item in items)
                {
                    if (File.Exists(item))
                    {
                        validFiles.Add(item);
                    }
                    if (Directory.Exists(item))
                    {
                        string[] files = Directory.GetFiles(item, "*.*", searchOption);

                        foreach (string file in files)
                        {
                            if (File.Exists (file)) validFiles.Add(file);
                        }
                    }
                }
            }

            // Filter by extension
            if (ext != null)
            {
                foreach (string file in validFiles)
                {
                    if (Path.GetExtension(file).ToLower() ==  ext.ToLower()) output.Add(file);
                }
            }
            else
            {
                output = validFiles;
            }

            return output;
        }

        /// <summary>
        /// Returns single directory if the user enters a valid path, returns null if the directory does not exist.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string? GetDirectory(string? defaultValue = null)
        {
            string type = "directory";

            string? output = defaultValue;

            UI.Input(type, defaultValue);

            string? input = Console.ReadLine();

            if (input != null && StandardCommands(input.ToString()))
            {
                input = null;
                output = null;
            }

            if (input != null) input = input.Replace("\"", "");

            if (Directory.Exists(input)) output = input;

            return output;
        }

        /// <summary>
        /// Enables ConsoleUI to perform its own commands instead of the calling application.
        /// Enter a command with a leading exclamation mark: "!command here"
        /// </summary>
        /// <param name="input"></param>
        static bool StandardCommands(string? input)
        {
            bool result = false;

            string logPath = AppDomain.CurrentDomain.BaseDirectory + "log.txt";

            switch (input)
            {
                case "!log":

                    ConsoleUIWrite("Attempting to open log file...");
                    if (File.Exists(logPath)) Process.Start("notepad.exe", logPath);
                    else ConsoleUIWrite("\"log.txt\" does not exist!");

                    result = true;
                    break;
                case "!logdel":

                    if (File.Exists(logPath))
                    {
                        File.Delete(logPath);
                        ConsoleUIWrite("Deleted \"log.txt\"");
                    }

                    result = true;
                    break;
                default:
                    break;
            }

            if (result) UI.Pause();

            return result;
        }

        static void StandardCommandAccepted(string command)
        {
            ConsoleUIWrite($"\"{command}\" recognised.");
            UI.Pause();
        }

        static void ConsoleUIWrite(string text)
        {
            Console.WriteLine($"ConsoleUI: {text}");
        }
    }
}
