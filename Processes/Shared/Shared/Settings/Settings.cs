using Newtonsoft.Json;
using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Settings
{
    public class Settings
    {
        //
        // Loads the current settings file
        //
        public static Options Load()
        {
            // Get the file and check it's there
            string settingsFile = string.Format(@"{0}\{1}", Paths.GetDataFolder(), SettingsFile);
            if (File.Exists(settingsFile) == false)
                return new Options();

            try
            {
                string settingsContent = File.ReadAllText(settingsFile);

                // Load the file
                Options loadedOptions = JsonConvert.DeserializeObject<Options>(settingsContent);
                return loadedOptions;
            }
            catch
            {
                // Empty options
                return new Options();
            }
        }

        //
        // Save the current settings file
        //
        public static void Save(Options options)
        {
            string jsonData = JsonConvert.SerializeObject(options);
            string settingsFile = string.Format(@"{0}\{1}", Paths.GetDataFolder(), SettingsFile);

            // Save it out
            File.WriteAllText(settingsFile, jsonData);
        }

        // Private properties
        private const string SettingsFile = "Settings";
    }
}
