using System.ComponentModel;
using System.Configuration;

namespace StackExchange.Opserver
{
    public class SettingsSection : ConfigurationSection
    {
        //[ConfigurationProperty("provider"), DefaultValue("JSONFile")]
        //public string Provider => this["provider"] as string;
        public string Provider { get; set; }
        //[ConfigurationProperty("name")]
        //public string Name => this["name"] as string;
        public string Name { get; set; }
        //[ConfigurationProperty("path")]
        //public string Path => this["path"] as string;
        public string Path { get; set; }
        [ConfigurationProperty("connectionString")]
        public string ConnectionString => this["connectionString"] as string;
    }
}