using System.Configuration;
using System.IO;
using System.Reflection;

namespace GitIgnore
{
    internal class ProgramConfiguration
    {
        internal const string SourceDirectoryConfigurationKey = "SourceDirectory";
        private readonly Configuration _configuration;

        public ProgramConfiguration()
        {
            _configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        }

        public string SourceDirectory
        {
            get => _configuration.AppSettings.Settings[SourceDirectoryConfigurationKey]?.Value;
            set
            {
                var settings = _configuration.AppSettings.Settings;
                settings.Remove(SourceDirectoryConfigurationKey);
                settings.Add(SourceDirectoryConfigurationKey, value);
                _configuration.Save();
            }
        }

        public bool IsValid => Directory.Exists(SourceDirectory);
    }
}
