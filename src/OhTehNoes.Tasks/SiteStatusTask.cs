using System;
using System.Xml;

namespace OhTehNoes.Tasks
{
    class SiteStatusTask : Task
    {
        public override string TaskName
        {
            get { return "SiteStatus"; }
        }

        public SiteStatusTask(Logger logger, XmlNode settings)
            : base(logger, settings)
        {

        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
