using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Tracker
{
    class Connection
    {
        static string connectionString = "server=getneural.com;user=tracker;database=tracker;port=3306;password=tracker@123#;sslmode=none";
        static MySqlConnection connection = null;

        public static MySqlConnection getConnection()
        {
            MySqlConnection c = Connection.connection;

            if (c != null)
            {
                return c;
            }
            try
            {
                c = new MySqlConnection(Connection.connectionString);
                c.Open();
            }
            catch (Exception ex)
            {
                c = null;
            }
            Connection.connection = c;

            return Connection.connection;

        }
        public static bool executeNonQuery (String qry)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(qry, Connection.getConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception e )
            {
                Console.Write(e.Message);
                return false;
            }
            return true;
        }
        public static void close()
        {
            if (Connection.connection != null)
            {
                try
                {
                    Connection.connection.Close();

                } catch (Exception e)
                {

                }
            }
        }
    }
}
