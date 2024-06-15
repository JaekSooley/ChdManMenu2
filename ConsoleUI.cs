namespace ConsoleUI
{
    public class UI
    {
        public static void Header(string header, bool clearPreviousScreen = true)
        {
            if (clearPreviousScreen) Console.Clear();

            int headerWidth = 48;
            int textWidth = header.Length + 2;
            int paddingWidth = (headerWidth - textWidth) / 2;
			string paddingString = "";
			for (int i = 0; i < paddingWidth; i++) paddingString += "=";

            string displayText = string.Empty;

            Console.WriteLine("");

            displayText += paddingString;

            displayText += $" {header.ToUpper()} ";

            displayText += paddingString;

            Console.WriteLine(displayText);
            Console.WriteLine("");
        }

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

        public static void Pause()
        {
            Console.WriteLine("");
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        public static void Error(string description)
        {
            Header("///// Error /////");
            Console.WriteLine(description);
            Pause();
        }

        public static void Warning(string description)
        {
            Header("Warning", false);
            Console.WriteLine(description);
        }

        // Yes, this is just because I can't be bothered typing Console.WriteLine() all the time.
        // No, I will not be taking questions.
        public static void Write(string text = "")
        {
            Console.WriteLine(text);
        }


        // Used by the Input class, don't call directly.
        public static void Input(string type, string? defaultValue = null)
        {
            Console.WriteLine("");
            if (defaultValue == null) Console.Write($"Input ({type}): "); 
            else Console.Write($"Input ({type}): [{defaultValue}] ");
        }
    }

    public class Input
    {
        public static bool? GetBoolean(bool? defaultValue = null)
        {
            string type = "bool";
            bool? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string input = ReadLine();

            if (bool.TryParse(input, out bool val)) output = val;

            return output;
        }
        public static int? GetInteger(int? defaultValue = null)
        {
            string type = "int";
            int? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string input = ReadLine();

            if (int.TryParse(input, out int val)) output = val;

            return output;
        }

        public static float? GetFloat(float? defaultValue = null)
        {
            string type = "float";
            float? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string input = ReadLine();

            if (float.TryParse(input, out float val)) output = val;

            return output;
        }

        public static double? GetDouble(double? defaultValue = null)
        {
            string type = "double";
            double? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string input = ReadLine();

            if (double.TryParse(input, out double val)) output = val;

            return output;
        }


        public static decimal? GetDecimal(decimal? defaultValue = null)
        {
            string type = "decimal";
            decimal? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue.ToString());
            else UI.Input(type);

            string input = ReadLine();

            if (decimal.TryParse(input, out decimal val)) output = val;

            return output;
        }

        public static string? GetString(string? defaultValue = null)
        {
            string type = "string";
            string? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue);
            else UI.Input(type);

            string input = ReadLine();

            if (input != "") output = input;

            return output;
        }

        public static string? GetFile(string? defaultValue = null)
        {
            string type = "file path";
            string? output = defaultValue;

            if (defaultValue != null) UI.Input(type, defaultValue);
            else UI.Input(type);

            string input = ReadLine().Replace("\"", "");

            if (File.Exists(input)) output = input;

            return output;
        }

        public static string? GetDirectory(string? defaultValue = null)
        {
            string type = "directory";
            string? output = defaultValue;

            UI.Input(type, defaultValue);

            string input = ReadLine().Replace("\"", "");

            if (Directory.Exists(input)) output = input;

            return output;
        }

        static string ReadLine()
        {
            string? input = Console.ReadLine();
            if (input == null) input = "";

            return input;
        }

        static void InputInvalid(string type, string input, string valueSetTo)
        {
            if (input == "") Console.WriteLine($"Default value \"{valueSetTo}\" used.");
            else Console.WriteLine($"Invalid input! Default value \"{valueSetTo}\" used.");
        }
    }
}
