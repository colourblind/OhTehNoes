using System;
using System.Collections.Generic;
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

            LoadPlugins(logger);

            try
            {
                if (args.Length > 0)
                    LoadTasks(logger, args[0]);
                else
                    LoadTasks(logger, null);
            }
            catch (FileNotFoundException ex)
            {
                logger.Write("Unable to load task file '" + ex.FileName + "'", Priority.Fatal);
            }

            foreach (Task task in Tasks)
                task.Run();
        }

        static void LoadPlugins(Logger logger)
        {
            Plugins = new Dictionary<string, Type>();

            DirectoryInfo pluginDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "plugins"));
            foreach (FileInfo file in pluginDirectory.GetFiles("*.dll"))
            {
                logger.Write(String.Format("Searching {0} for plugins", file.Name), Priority.Debug);
                Assembly assembly = Assembly.LoadFile(file.FullName);
                foreach (Type type in assembly.GetTypes())
                {
                    object[] attributes = type.GetCustomAttributes(typeof(TaskAttribute), true);
                    if (attributes != null)
                    {
                        foreach (object attribute in attributes)
                        {
                            if (attribute is TaskAttribute)
                            {
                                logger.Write(String.Format("Loading plugin {0}", type.FullName), Priority.Debug);
                                TaskAttribute taskAttribute = (TaskAttribute)attributes[0];
                                Plugins[taskAttribute.TypeName] = type;
                            }
                        }
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
                if (Plugins.ContainsKey(node.Attributes["type"].Value))
                {
                    Type pluginType = Plugins[node.Attributes["type"].Value];
                    object[] args = new object[] { logger, node };
                    try
                    {
                        Task task = (Task)Activator.CreateInstance(pluginType, args);
                        Tasks.Add(task);
                    }
                    catch (Exception e)
                    {
                        logger.Write(String.Format("Error creating {0} (try checking your arguments!)", pluginType.Name), e);
                    }
                }
                else
                    logger.Write("Unable to find '" + node.Attributes["type"].Value + "' plugin", Priority.Error);
            }
        }
    }
}
