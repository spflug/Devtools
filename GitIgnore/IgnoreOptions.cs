using CommandLine;

namespace GitIgnore
{
    [Verb("select", HelpText = "creates the desired ignore file in the current directory.", Hidden = false)]
    internal class IgnoreOptions
    {
        [Option('p', "pattern", HelpText = "The search pattern to find the desired ignore file.", Required = true)]
        public string SearchPattern { get; set; }

        [Option('f', "force", HelpText = "The selection will override any existing ignore file when this switch is set.", Required = false, Default = false)]
        public bool ForceOverride { get; set; }
    }
}
