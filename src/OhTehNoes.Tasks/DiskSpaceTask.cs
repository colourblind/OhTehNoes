using System;
using System.IO;
using System.Xml;

namespace OhTehNoes.Tasks
{
    class DiskSpaceTask : Task
    {
        public override string TaskName
        {
            get { return "DiskSpace"; }
        }

        private long WarningThreshold
        {
            get;
            set;
        }

        public DiskSpaceTask(Logger logger, XmlNode settings)
            : base(logger, settings)
        {
            if (settings.Attributes["warningThreshold"] == null)
                throw new ArgumentNullException("warningThreshold");

            WarningThreshold = long.Parse(settings.Attributes["warningThreshold"].Value) * 1024 * 1024;
        }

        public override void Run()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    if (drive.AvailableFreeSpace < WarningThreshold)
                    {
                        Logger.Write(String.Format("{0:n0} MB left on drive: {1}", drive.AvailableFreeSpace / (1024 * 1024), drive.Name), Priority.Warn);
                    }
                }
            }
        }
    }
}
