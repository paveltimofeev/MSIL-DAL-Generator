using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace dalCoreSE
{
    public class SQLConfigurator
    {
        static SqlConnection conn = null;

        private SQLConfigurator(string connString)
        {
            SQLConfigurator.conn = new SqlConnection(connString);
        }

        public static SqlConnection GetConnection()
        {
            if (conn != null)
                return conn;
            else
                throw new Exception("Connection string was not configured");
        }

        public static void InitializeConnection(string connString)
        {
            SQLConfigurator.conn = new SqlConnection(connString);
        }
    }
}
