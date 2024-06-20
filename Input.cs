using System.Diagnostics;
using System.Text;

namespace ConsoleUI
{
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
                            if (File.Exists(file)) validFiles.Add(file);
                        }
                    }
                }
            }

            // Filter by extension
            if (ext != null)
            {
                foreach (string file in validFiles)
                {
                    if (Path.GetExtension(file).ToLower() == ext.ToLower()) output.Add(file);
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
