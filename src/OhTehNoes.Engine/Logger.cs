﻿using System;
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

        public Logger() : this("OhTehNoes")
        {

        }

        public Logger(string logName)
        {
            Log = LogManager.GetLogger(logName);
            XmlConfigurator.Configure();
        }

        public void Write(Task task, string message, Priority priority)
        {
            Write(String.Format("{0}{1}", String.IsNullOrEmpty(task.Name) ? String.Empty : task.Name + " - ", message), priority);
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
