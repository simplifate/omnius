using System;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;

namespace Logger
{
    internal class GeneralConfig : ConfigurationSection
    {

        // For easier access to config
        private static GeneralConfig config = (GeneralConfig)ConfigurationManager.GetSection("logger");
        internal static GeneralConfig Config
        {
            get
            {
                return config;
            }
        }

        // Create a "rootDir" attribute
        [ConfigurationProperty("rootDir", DefaultValue = @"C:\Log\Logger\", IsRequired = false)]
        public String RootDir
        {
            get
            {
                return (String)base["rootDir"];
            }
        }

        // Create a "settings" element
        [ConfigurationProperty("settings")]
        public SettingsElementCollection Settings
        {
            get
            {
                return (SettingsElementCollection)base["settings"];
            }
        }

    }

    // Define the "settings" element that contains collection of "log" elements
    // overrides the default "add" tag to "log"
    [ConfigurationCollection(typeof(LogElement), AddItemName = "log", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    internal class SettingsElementCollection : ConfigurationElementCollection
    {
        public new ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        // method for elementName must be overriden to show correct name
        protected override string ElementName
        {
            get
            {
                return "log";
            }
        }

        // method for creating new element must be overriden to create LogElements
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogElement();
        }

        // method for getting element key must be overriden to get correct LogElement key according to its Name to serve as key
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogElement)element).Name;
        }

        // Basic methods for working with elements in collection
        public void Remove(LogElement serviceConfig)
        {
            BaseRemove(serviceConfig.Name);
        }
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
        public void Remove(String name)
        {
            BaseRemove(name);
        }
        public void Add(LogElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }
        public void Clear()
        {
            BaseClear();
        }

        // this must be overloaded to show correct indexing of LogElements
        public LogElement this[int index]
        {
            get
            {
                return (LogElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
        
        // overloaded this to access "log" element config via LogType parameter
        public LogElement this[Log.Type name]
        {
            get
            {
                return (LogElement)BaseGet(name);                
            }
        }
        
        // overloaded this to access "log" element config via string
        public new LogElement this[string name]
        {
            get
            {
                Log.Type logType = (Log.Type)Enum.Parse(typeof(Log.Type), name);
                return (LogElement)BaseGet(logType);
            }
        }
    }

    // Define the "log" element
    // with "name", "dir", "file" and "save" attributes.
    internal class LogElement : ConfigurationElement
    {
        //  attribute name is unique key that can be only string that is same as enum LogType
        [ConfigurationProperty("name", IsRequired = true)]
        public Log.Type? Name
        {
            get
            {
                return base["name"] as Log.Type?;
            }
        }
        [ConfigurationProperty("dir", IsRequired = false)]
        public String Dir
        {
            get
            {
                return (String)base["dir"];
            }
        }

        // file name of log file attribute not neccessary should be log.txt
        [ConfigurationProperty("file", DefaultValue = "log.txt", IsRequired = false)]
        public String File
        {
            get
            {
                return (String)base["file"];
            }
        }

        // attribute that says if log should be saved - be written
        [ConfigurationProperty("save", DefaultValue = true, IsRequired = false)]
        public Boolean Save
        {
            get
            {
                return (Boolean)base["save"];
            }

        }

        // attribute to set max size of log file in Mega Bytes
        [ConfigurationProperty("maxLogSize", DefaultValue = 20, IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 100, MinValue = 1)]
        public int MaxLogSize
        {
            get
            { return (int)base["maxLogSize"]; }
            set
            { base["maxLogSize"] = value; }
        }
    }
}
