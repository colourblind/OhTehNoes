using System;
using System.ComponentModel;
using System.Xml;
using log4net;

namespace OhTehNoes
{
    public abstract class Task
    {
        protected Logger Logger
        {
            get;
            private set;
        }

        protected XmlNode Settings
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                if (Settings.Attributes["name"] == null)
                    return String.Empty;
                else
                    return Settings.Attributes["name"].Value;
            }
        }

        protected Task(Logger logger, XmlNode settings)
        {
            Logger = logger;
            Settings = settings;
        }

        public abstract void Run();
    }

    public class TaskAttribute : Attribute
    {
        public string TypeName
        {
            get;
            set;
        }

        public TaskAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}
