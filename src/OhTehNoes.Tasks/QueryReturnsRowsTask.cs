using System;
using System.Data.SqlClient;
using System.Xml;

namespace OhTehNoes.Tasks
{
    [Task("QueryReturnsRows")]
    class QueryReturnsRowsTask : Task
    {
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

        public QueryReturnsRowsTask(Logger logger, XmlNode settings) : base(logger, settings)
        {
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
                command.Connection.Open();
                reader = command.ExecuteReader();

                if (!reader.HasRows)
                    Logger.Write(this, String.Format("Query '{0}' returned no rows!", Name), Priority.Warn);
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
