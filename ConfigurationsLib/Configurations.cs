using System;
using System.Collections.Generic;
using System.Text;

namespace Librac.ConfigurationsLib
{
    /// <summary>
    /// Provides methods for loading and saving configuration settings to and from a file.
    /// </summary>
    public static class Configurations
    {
        private static ConfigurationsMethods _configurationMethods = new ConfigurationsMethods();

        /// <summary>
        /// Loads configuration settings from a specified file.
        /// </summary>
        /// <param name="fullFileName">The path to the configuration file from which to load settings.</param>
        /// <returns>
        /// A dictionary containing configuration settings if the file exists and is properly formatted;
        /// otherwise, null if the file does not exist or an error occurs during loading.
        /// </returns>
        public static Dictionary<string, string>? LoadConfiguration(string fullFileName)
        {
            return _configurationMethods.LoadConfigurationFromFile(fullFileName);
        }

        /// <summary>
        /// Saves configuration settings to a specified file.
        /// </summary>
        /// <param name="dictionary">A dictionary containing the configuration settings to save.</param>
        /// <param name="fullFileName">The path to the configuration file where the settings should be saved.</param>
        /// <remarks>
        /// This method writes the configuration settings from the provided dictionary to the file specified by <paramref name="fullFileName"/>.
        /// If the file does not exist, it will be created; if it does exist, it will be overwritten.
        /// </remarks>
        public static void SaveConfiguration(Dictionary<string, string> dictionary, string fullFileName)
        {
            _configurationMethods.SaveConfigurationToFile(dictionary, fullFileName);
        }
    }

}
