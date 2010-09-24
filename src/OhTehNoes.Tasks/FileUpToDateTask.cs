using System;
using System.IO;
using System.Xml;

namespace OhTehNoes.Tasks
{
    [Task("FileUpToDate")]
    class FileUpToDateTask : Task
    {
        public string Filename
        {
            get;
            set;
        }

        public int ThresholdInMinutes
        {
            get;
            set;
        }

        public FileUpToDateTask(Logger logger, XmlNode settings)
            : base(logger, settings)
        {
            Filename = settings.Attributes["filename"].Value;
            ThresholdInMinutes = Int32.Parse(settings.Attributes["thresholdInMinutes"].Value);
        }

        public override void Run()
        {
            FileInfo file = new FileInfo(Filename);
            DateTime lastEditTime = file.LastWriteTime;

            int fileAgeInMinutes = Convert.ToInt32(DateTime.Now.Subtract(lastEditTime).TotalMinutes);

            if (fileAgeInMinutes > ThresholdInMinutes)
                Logger.Write(this, String.Format("File '{0}' is out of date! Theshold(m): {1}, Age(m): {2}", Filename, ThresholdInMinutes, fileAgeInMinutes), Priority.Warn);
        }
    }
}
