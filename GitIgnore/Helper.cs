using System;

namespace GitIgnore
{
    internal static class Helper
    {
        internal static bool TryGetConfiguration(out ProgramConfiguration config)
        {
            config = new ProgramConfiguration();
            if (!config.IsValid)
            {
                Print(
                    "source directory not found or source directory configuration is missing.\n" +
                    "Run 'config --directory <path-to-source>' command to set the source directory path.",
                    ConsoleColor.Red);
            }

            return config.IsValid;
        }

        internal static int Print(string message, ConsoleColor color)
        {
            var tmp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = tmp;
            return 0;
        }
    }
}