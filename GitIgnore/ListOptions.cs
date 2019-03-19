using CommandLine;

namespace GitIgnore
{
    [Verb("list", HelpText = "displays a list of possible git ignore settings.", Hidden = false)]
    internal class ListOptions
    {
        [Option('p', "pattern", Default= "*.gitignore", HelpText = "an optional search pattern.", Required = false)]
        public string SearchPattern { get; set; }
    }
}
