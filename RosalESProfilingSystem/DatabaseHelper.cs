using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosalESProfilingSystem
{
    class DatabaseHelper
    {
        private static string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
