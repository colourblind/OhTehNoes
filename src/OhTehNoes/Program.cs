using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace OhTehNoes
{
    class Program
    {
        static List<Task> _taskList;

        static void Main(string[] args)
        {
            Logger logger = new Logger();

            if (args.Length > 0)
                LoadTasks(logger, args[0]);
            else
                LoadTasks(logger, null);

            foreach (Task task in _taskList)
                task.Run();
        }

        static void LoadTasks(Logger logger, string taskFilename)
        {
            _taskList = new List<Task>();

            if (String.IsNullOrEmpty(taskFilename))
                taskFilename = "tasks.xml";

            XmlDocument settings = new XmlDocument();
            settings.Load(taskFilename);

            foreach (XmlNode node in settings.DocumentElement.ChildNodes)
                _taskList.Add(LoadPlugin(node.Attributes["assemblyName"].Value, node.Attributes["typeName"].Value, logger, node));
        }

        static Task LoadPlugin(string assemblyName, string typeName, Logger logger, XmlNode taskSettings)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            Type pluginType = assembly.GetType(typeName, true, true);

            object[] args = new object[] { logger, taskSettings };

            return (Task)assembly.CreateInstance(typeName, true, BindingFlags.Instance | BindingFlags.Public, null, args, CultureInfo.CurrentCulture, null);
        }
    }
}
