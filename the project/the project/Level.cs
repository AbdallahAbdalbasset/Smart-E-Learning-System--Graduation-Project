using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace the_project
{
    class Level
    {
        public int levelnumber;
        public string levelname;
        public List<Subject> subjectsnames = new List<Subject>();
        //database variable
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public Level(int id)
        {
            server = "mysql-81913-0.cloudclusters.net";
            database = "Faculty";
            uid = "admin";
            password = "1SOin0zW";
            string connectionString;
            connectionString = "SERVER=" + server + "; PORT = 19902 ;" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
            this.levelnumber = id;
            string query = "select * from subjects where level_id=" + id;
            this.connection.Open();

            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            Subject temp = new Subject();
            //Read the data and store them in the list
            while (dataReader.Read())
            {
                temp = new Subject();
                temp.name = (string)dataReader["subject_name"];
                temp.subject_id = (int)dataReader["subject_id"];


                this.subjectsnames.Add(temp);
            }
            dataReader.Close();
            for (int i = 0; i < this.subjectsnames.Count; i++)
            {
                query = "select file_name from files where subject_id=" + subjectsnames[i].subject_id;
                //cmd = new MySqlCommand(query);

                //cmd.ExecuteReader();
                MySqlCommand cm = new MySqlCommand(query, connection);
                MySqlDataReader dataR = cm.ExecuteReader();
                while (dataR.Read())
                {
                    subjectsnames[i].books.Add((string)dataR["file_name"]);
                }
                dataR.Close();
            }

            this.connection.Close();

        }



    }
}
