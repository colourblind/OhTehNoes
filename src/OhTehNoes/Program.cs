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
            Logger logger = null;
            XmlDocument taskFile = null;

            try
            {
                string taskFilename = "tasks.xml";
                if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
                    taskFilename = args[0];

                taskFile = new XmlDocument();
                taskFile.Load(taskFilename);
            }
            catch (FileNotFoundException ex)
            {
                logger = new Logger();
                logger.Write("Unable to load task file '" + ex.FileName + "'", Priority.Fatal);
                return;
            }

            if (taskFile.DocumentElement.Attributes["logName"] == null)
                logger = new Logger();
            else
                logger = new Logger(taskFile.DocumentElement.Attributes["logName"].Value);

            try
            {
                LoadPlugins(logger);
                LoadTasks(logger, taskFile);

                foreach (Task task in Tasks)
                    task.Run();
            }
            catch (Exception ex)
            {
                logger.Write("Ruh roh! Uncaught exception!", ex);
            }
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
                    if (type.IsSubclassOf(typeof(Task)) && attributes != null)
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

        static void LoadTasks(Logger logger, XmlDocument taskFile)
        {
            Tasks = new List<Task>();

            foreach (XmlNode node in taskFile.DocumentElement.ChildNodes)
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
                    catch (Exception)
                    {
                        if (node.Attributes["name"] == null)
                            logger.Write(String.Format("Error creating unnamed task of type {0} (try checking the attributes in your taskfile!)", pluginType.Name), Priority.Error);
                        else
                            logger.Write(String.Format("Error creating '{1}' of type {0} (try checking the attributes in your taskfile!)", pluginType.Name, node.Attributes["name"].Value), Priority.Error);
                    }
                }
                else
                    logger.Write("Unable to find '" + node.Attributes["type"].Value + "' plugin", Priority.Error);
            }
        }
    }
}
