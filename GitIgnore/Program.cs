using System;
using System.Collections.Generic;
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
                    try
                    {
                        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);

                        Print(
                            "more than one file found." +
                            "\nPlease select the file to be copied using the ArrowUp and ArrowDown key\n" +
                            "and press enter to copy the selected file",
                            ConsoleColor.Yellow);
                    }
                    catch (IOException)
                    {
                        // ignore it => special treatment when no console window present
                    }

                    var foregroundColor = Console.ForegroundColor;
                    var counter = 0;
                    while (true)
                    {
                        var current = files[counter % files.Count];

                        try
                        {
                            Console.SetCursorPosition(0, Console.CursorTop);
                        }
                        catch (IOException)
                        {
                            var selection = SelectFromNumberedList();
                            if (!(selection is null)) Copy(selection);
                            return 0;
                        }

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

            FileInfo SelectFromNumberedList()
            {
                foreach (var entry in files.Select((f, i) => $"{i + 1,3:d}: {f.Value.Name}"))
                {
                    Print(entry, ConsoleColor.White);
                }

                Console.Write("\nInput the number you want to ignore: ");
                var digits = new List<int>();
                while (true)
                {
                    var read = Console.Read();
                    if (read < 0) return default;

                    switch (Convert.ToChar(read))
                    {
                        case char key when char.IsDigit(key):
                            digits.Add(key % 48);
                            break;
                        case char key when key.In('\n', '\r'):
                            var selection = digits
                                .Select((value, index) => (value, index))
                                .Aggregate(0, (accumulator, value) => accumulator + (int)Math.Pow(10, value.index) * value.value)
                                - 1;
                            if (files.ContainsKey(selection)) return files[selection];

                            Print($"Can not find entry {selection + 1}", ConsoleColor.Red);
                            return SelectFromNumberedList();
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

            void Copy(FileInfo file)
            {
                try
                {
                    Print($"copying {file.Name} to .gitignore", ConsoleColor.Cyan);
                    file.CopyTo(".gitignore", false);
                }
                catch (IOException)
                {
                    Print("A .gitignore file already exists.\nUse the --force (-p) switch to force an override.", ConsoleColor.Red);
                    Print(Environment.CommandLine + " --force", ConsoleColor.Yellow);
                }
            }

            return 0;
        }

        private static int Print(string message, ConsoleColor color)
        {
            var tmp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = tmp;
            return 0;
        }
    }
}
