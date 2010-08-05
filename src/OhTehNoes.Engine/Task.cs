using System;
using System.Xml;
using log4net;

namespace OhTehNoes
{
    public abstract class Task
    {
        public abstract string TaskName
        {
            get;
        }

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

        protected Task(Logger logger, XmlNode settings)
        {
            Logger = logger;
            Settings = settings;
        }

        public abstract void Run();
    }
}
