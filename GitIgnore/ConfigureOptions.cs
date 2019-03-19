using CommandLine;

namespace GitIgnore
{
    [Verb("config", HelpText = "gets or sets program options")]
    internal class ConfigureOptions
    {
        [Option('d', "directory", Default = "./", HelpText = "sets the source directory")]
        public string SourceDirectory { get; set; }

        [Option('s', "show", Default = false, HelpText = "displays the configuration")]
        public bool Show { get; set; }
    }
}
