using System;
using System.Data.SqlClient;
using System.Xml;

namespace OhTehNoes.Tasks
{
    class QueryReturnsRowsTask : Task
    {
        public override string TaskName
        {
            get { return "QueryReturnsRows"; }
        }

        private string Name
        {
            get;
            set;
        }

        private string ConnectionString
        {
            get;
            set;
        }

        private string SqlQuery
        {
            get;
            set;
        }

        protected QueryReturnsRowsTask(Logger logger, XmlNode settings) : base(logger, settings)
        {
            Name = settings.Attributes["Name"].Value;
            ConnectionString = settings.Attributes["connectionString"].Value;
            SqlQuery = settings.Attributes["sqlQuery"].Value;
        }

        public override void Run()
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection = new SqlConnection(ConnectionString);
                command = new SqlCommand(SqlQuery, connection);
                reader = command.ExecuteReader();

                if (!reader.HasRows)
                    Logger.Write(String.Format("Query '{0}' returned no rows!", Name), Priority.Warn);
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
                if (command != null)
                    command.Dispose();
                if (connection != null)
                    connection.Dispose();
            }
        }
    }
}
