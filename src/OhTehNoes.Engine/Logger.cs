using System;
using log4net;
using log4net.Config;

namespace OhTehNoes
{
    public class Logger
    {
        private ILog Log
        {
            get;
            set;
        }

        public Logger()
        {
            Log = LogManager.GetLogger("OhTehNoes");
            XmlConfigurator.Configure();
        }

        public void Write(string message, Priority priority)
        {
            switch (priority)
            {
                case Priority.Debug:
                    Log.Debug(message);
                    break;
                case Priority.Info:
                    Log.Info(message);
                    break;
                case Priority.Warn:
                    Log.Warn(message);
                    break;
                case Priority.Error:
                    Log.Error(message);
                    break;
                case Priority.Fatal:
                    Log.Fatal(message);
                    break;
            }
        }

        public void Write(string message, Exception e)
        {
            Log.Error(message, e);
        }
    }

    public enum Priority
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
