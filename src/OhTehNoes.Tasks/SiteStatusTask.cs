using System;
using System.Xml;

namespace OhTehNoes.Tasks
{
    [Task("SiteStatus")]
    class SiteStatusTask : Task
    {
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
