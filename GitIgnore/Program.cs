using System;
using System.IO;
using System.Linq;
using CommandLine;
using static GitIgnore.Helper;

namespace GitIgnore
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<ConfigureOptions, ListOptions, IgnoreOptions>(args)
                .WithNotParsed(errors => errors.ForEach(Console.WriteLine))
                .MapResult(
                    (ConfigureOptions options) => Configure(options),
                    (ListOptions options) => List(options),
                    (IgnoreOptions options) => Ignore(options),
                    errors => 1);
        }

        private static int Configure(ConfigureOptions options)
        {
            var configuration = new ProgramConfiguration();
            if (options.Show)
            {
                var setting = configuration.SourceDirectory;
                if (setting is null) return Print("no source directory defined", ConsoleColor.DarkYellow);
                var directory = new DirectoryInfo(setting);
                Console.WriteLine(ProgramConfiguration.SourceDirectoryConfigurationKey + $" = {setting}");
                if (!directory.Exists)
                {
                    return Print("directory not found", ConsoleColor.Yellow);
                }
            }
            else
            {
                var directory = new DirectoryInfo(options.SourceDirectory);
                Console.WriteLine(ProgramConfiguration.SourceDirectoryConfigurationKey + $" = {options.SourceDirectory}");
                if (!directory.Exists)
                {
                    return Print("directory not found", ConsoleColor.Red);
                }

                configuration.SourceDirectory = directory.FullName;
                return Print("source directory configured", ConsoleColor.Green);
            }

            return 0;
        }

        private static int List(ListOptions options)
        {
            if (!TryGetConfiguration(out var configuration)) return 1;

            var directory = new DirectoryInfo(configuration.SourceDirectory);
            Console.WriteLine($"Looking for {options.SearchPattern} in {directory.FullName}");
            foreach (var file in directory.EnumerateFiles(options.SearchPattern))
            {
                Print($" -> {file}", ConsoleColor.Gray);
            }

            return 0;
        }

        private static int Ignore(IgnoreOptions options)
        {
            if (!TryGetConfiguration(out var configuration))
                return 1;

            var directory = new DirectoryInfo(configuration.SourceDirectory);
            Console.WriteLine($"Looking for {options.SearchPattern} in {directory.FullName}");

            var files = directory
                .EnumerateFiles(options.SearchPattern)
                .Select((f, i) => (i, f))
                .ToDictionary(t => t.i, t => t.f);

            switch (files.Count)
            {
                case 0: return Print("no file found", ConsoleColor.Red) + 1;
                case 1:
                    Copy(files[0]);
                    break;
                default:
                    Print(
                        "more than one file found." +
                        "\nPlease select the file to be copied using the ArrowUp and ArrowDown key\n" +
                        "and press enter to copy the selected file",
                        ConsoleColor.Yellow);

                    var foregroundColor = Console.ForegroundColor;
                    var current = new FileInfo("foo");
                    var counter = 0;
                    while (true)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.ForegroundColor = Console.BackgroundColor;
                        Console.Write(current.Name);

                        current = files[counter % files.Count];

                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.ForegroundColor = foregroundColor;
                        Console.Write(current.Name);

                        switch (ReadNavigationKey().Key)
                        {
                            case ConsoleKey.Enter:
                                Copy(current);
                                return 0;
                            case ConsoleKey.DownArrow:
                                if (counter > 0) counter--;
                                continue;
                            case ConsoleKey.UpArrow:
                                counter++;
                                continue;
                        }
                    }
            }

            ConsoleKeyInfo ReadNavigationKey()
            {
                while (true)
                {
                    switch (Console.ReadKey(true))
                    {
                        case ConsoleKeyInfo key when key.Key.In(ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.UpArrow):
                            return key;
                    }
                }
            }

            void Copy(FileInfo file) => file.CopyTo(".gitignore");

            return 0;
        }
    }
}
