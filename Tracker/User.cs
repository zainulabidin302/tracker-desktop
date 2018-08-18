using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Tracker
{
    class User
    {
        int id;
        String username;
        String email;
        bool loggedIn = false;

        public string Username { get => username; set => username = value; }
        public string Email { get => email; set => email = value; }
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }
        public int Id { get => id; set => id = value; }

        public bool login(String name, String password)
        {
            try
            {
             
                string sql = "SELECT * from user where username = '"+ name +"' and password =  '"+ password +"'";
                MySqlCommand cmd = new MySqlCommand(sql, Connection.getConnection());
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Id = (int)rdr[0];
                    Username = rdr[1].ToString();
                    Email = rdr[2].ToString();
                }
                LoggedIn = true;
                rdr.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggedIn = false;
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        bool startTracking()
        {
            if (LoggedIn)
            {
                try
                {
                    string sql = "INSERT INTO timing VALUES (NULL, 'START', " + Id + ", NULL)";
                    MySqlCommand cmd = new MySqlCommand(sql, Connection.getConnection());
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Username = rdr[1].ToString();
                        Email = rdr[2].ToString();
                    }
                    LoggedIn = true;
                    rdr.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return false;
        }
    }
}
