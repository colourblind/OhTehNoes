using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace OhTehNoes
{
    static class Program
    {
        static List<Task> Tasks
        {
            get;
            set;
        }

        static Dictionary<string, Type> Plugins
        {
            get;
            set;
        }

        static void Main(string[] args)
        {
            Logger logger = new Logger();

            LoadPlugins();

            if (args.Length > 0)
                LoadTasks(logger, args[0]);
            else
                LoadTasks(logger, null);

            foreach (Task task in Tasks)
                task.Run();
        }

        static void LoadPlugins()
        {
            Plugins = new Dictionary<string, Type>();

            DirectoryInfo pluginDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "plugins"));
            foreach (FileInfo file in pluginDirectory.GetFiles("*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(file.FullName);
                foreach (Type type in assembly.GetTypes())
                {
                    object[] attributes = type.GetCustomAttributes(typeof(TaskAttribute), true);
                    if (attributes != null)
                    {
                        TaskAttribute taskAttribute = (TaskAttribute)attributes[0];
                        Plugins[taskAttribute.Name] = type;
                    }
                }
            }
        }

        static void LoadTasks(Logger logger, string taskFilename)
        {
            Tasks = new List<Task>();

            if (String.IsNullOrEmpty(taskFilename))
                taskFilename = "tasks.xml";

            XmlDocument settings = new XmlDocument();
            settings.Load(taskFilename);

            foreach (XmlNode node in settings.DocumentElement.ChildNodes)
            {
                if (Plugins.ContainsKey(node.Attributes["name"].Value))
                {
                    Type pluginType = Plugins[node.Attributes["name"].Value];
                    object[] args = new object[] { logger, node };
                    Task task = (Task)Activator.CreateInstance(pluginType, args);
                    Tasks.Add(task);
                }
                else
                    logger.Write("Unable to find '" + node.Attributes["name"].Value + "' plugin", Priority.Error);
            }
        }
    }
}
