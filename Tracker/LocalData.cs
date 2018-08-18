using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Net;

namespace Tracker
{
    class LocalData
    {
        SQLiteConnection connection = null;
        private const string TEMP_FILE_NAME = "SLHFSKUEHFROAIWHDKLUHDSFKUHSDKCNB";
        private string databaseSource;

        public string DatabaseSource { get => databaseSource; set => databaseSource = value; }

   
        public void InitStructure()
        {
            
            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = @"CREATE TABLE IF NOT EXISTS [timing] (
                        [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [event] TEXT NOT NULL,
                        [user_id] INTEGER NOT NULL,
                        [time] timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
                        )";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE IF NOT EXISTS [activity] (
                        [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [type] TEXT NOT NULL,
                        [time] timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        [user_id] INTEGER NOT NULL
                        )";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE IF NOT EXISTS [screenshots] (
                        [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [filename] TEXT NOT NULL,
                        [time] timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        [user_id] INTEGER NOT NULL
                        )";
            command.ExecuteNonQuery();

        }

        public SQLiteConnection getConnection()
        {
            if (connection != null) return connection;
            
            try
            {
                var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var path = Path.Combine(directory, "Tracker", TEMP_FILE_NAME);
                DatabaseSource = "data source=" + path ;

                if (!File.Exists(path))
                {
                    SQLiteConnection.CreateFile(path);
                    connection = new SQLiteConnection(DatabaseSource);
                    connection.Open();
                    InitStructure();
                } else
                {
                    connection = new SQLiteConnection(DatabaseSource);
                    connection.Open();
                }

                return connection;
            } catch(Exception e)
            {
                Console.Write(e.Message);
            }
            return null;
        }
        public bool syncTiming(User u) {
            SQLiteConnection con = getConnection();
            SQLiteCommand command = new SQLiteCommand(con);
            command.CommandText = @"SELECT * from timing where user_id = " + u.Id + "";
            String qry = @"INSERT INTO timing VALUES ";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    qry += " (NULL, '" + reader[1] + "', " + reader[2] + ", '" + DateTime.Parse(reader[3].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'),";
                }
            }
            qry = qry.Substring(0, qry.Length - 1);
            bool done = Connection.executeNonQuery(qry);
            if (done)
            {
                command.CommandText = @"DELETE from timing where user_id = " + u.Id + "";
                command.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        public bool syncActivity(User u) {
            SQLiteConnection con = getConnection();
            SQLiteCommand command = new SQLiteCommand(con);
            command.CommandText = @"SELECT * from activity where user_id = " + u.Id + "";
            String qry = @"INSERT INTO activity VALUES ";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    qry += " (NULL, '" + reader[1] + "', '" + DateTime.Parse(reader[2].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'," + reader[3] + "),";
                }
            }
            qry = qry.Substring(0, qry.Length - 1);
            bool done = Connection.executeNonQuery(qry);
            if (done)
            {
                command.CommandText = @"DELETE from activity where user_id = " + u.Id + "";
                command.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        public bool syncScreenShots(User u) {


            SQLiteConnection con = getConnection();
            SQLiteCommand command = new SQLiteCommand(con);
            command.CommandText = @"SELECT * from screenshots where user_id = " + u.Id + "";
            String qry = @"INSERT INTO screenshots VALUES ";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        WebClient client = new WebClient();
                        string fileToUpload = @"C:/Users/zain3/Source/Repos/Tracker/Tracker/bin/Debug/" + reader[1];
                        Console.Write(fileToUpload);
                        client.Credentials = CredentialCache.DefaultCredentials;
                        byte[] b = client.UploadFile(@"http://getneural.com/fileupload.php", "POST", fileToUpload);
                        string myString = System.Text.Encoding.ASCII.GetString(b).Trim();
                        
                        client.Dispose();
                    }
                    catch (Exception err)
                    {
                        Console.Write(err.Message);
                    }
                    qry += " (NULL, '" + reader[1] + "', '" + DateTime.Parse(reader[2].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'," + reader[3] + "),";
                }
            }
            qry = qry.Substring(0, qry.Length - 1);
            bool done = Connection.executeNonQuery(qry);
            if (done)
            {
                command.CommandText = @"DELETE from screenshots where user_id = " + u.Id + "";
                command.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        public bool sync(User u)
        {
            try
            {
                syncActivity(u);
                syncTiming(u);
                syncScreenShots(u);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
            return true;
        }
        public bool startTracking(User u)
        {
            try
            {
                SQLiteConnection con = getConnection();
                SQLiteCommand command = new SQLiteCommand(con);
                command.CommandText = @"INSERT INTO timing (id, event, user_id) VALUES (NULL, 'START', " + u.Id + ")";
                command.ExecuteNonQuery();
            } catch(Exception e) {
                Console.Write(e.Message);
                return false;
            }
            return true;
        }

        public bool trackActivity (string type, User u)
        {
            return executeNonQuery("Insert into activity (id, type, user_id) VALUES (NULL, '" + type + "', " + u.Id + ")");
        }

        public bool trackFile(string filename,  User u)
        {
            return executeNonQuery("Insert into screenshots (id, filename, user_id) VALUES (NULL, '" + filename + "', " + u.Id + ")");
        }

        public bool executeNonQuery(string query)
        {
            try
            {
                SQLiteConnection con = getConnection();
                SQLiteCommand command = new SQLiteCommand(con);
                command.CommandText = @query;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
            return true;
        }

        public bool endTracking(User u)
        {
            try
            {
                SQLiteConnection con = getConnection();
                SQLiteCommand command = new SQLiteCommand(con);
                command.CommandText = @"INSERT INTO timing (id, event, user_id) VALUES (NULL, 'END', " + u.Id + ")";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
            return true;
        }


    }
}
