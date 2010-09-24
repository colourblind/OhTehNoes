using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace OhTehNoes.Tasks
{
    [Task("SslCertificate")]
    class SslCertificateTask : Task
    {
        private int WarningThresholdInDays
        {
            get;
            set;
        }

        private List<string> CertsToCheck
        {
            get;
            set;
        }

        public SslCertificateTask(Logger logger, XmlNode settings)
            : base(logger, settings)
        {
            WarningThresholdInDays = Int32.Parse(settings.Attributes["warningThreshold"].Value);
            CertsToCheck = settings.Attributes["certsToCheck"].Value.Split(new char[] { ',', ';' }).ToList<string>(); // Take THAT, Law of Demeter
        }

        public override void Run()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            foreach (string certName in CertsToCheck)
            {
                X509Certificate2Collection results = store.Certificates.Find(X509FindType.FindBySubjectName, certName, false);
                if (results.Count == 0)
                    Logger.Write(this, String.Format("Certificate for {0} missing!", certName), Priority.Warn);
                else
                {
                    double daysRemaining = Double.MinValue;
                    foreach (X509Certificate2 cert in results)
                        daysRemaining = Math.Max(daysRemaining, cert.NotAfter.Subtract(DateTime.Now).TotalDays);

                    if (daysRemaining < WarningThresholdInDays)
                        Logger.Write(this, String.Format("Certificate for {0} only has {1:n0} days remaining!", certName, daysRemaining), Priority.Warn);
                }
            }

            store.Close();
        }
    }
}
