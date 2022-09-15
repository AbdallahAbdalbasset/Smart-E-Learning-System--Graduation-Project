using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace the_project
{
    public partial class AddingSubject : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public AddingSubject()
        {
            InitializeComponent();
            server = "mysql-81913-0.cloudclusters.net";
            database = "Faculty";
            uid = "admin";
            password = "1SOin0zW";
            string connectionString;
            connectionString = "SERVER=" + server + "; PORT = 19902 ;" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private void button2_Click(object sender, EventArgs e)
        {
           this.Dispose();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = "insert into subjects (level_id,subject_name) Values (@level_id,@subject_name)";

            //open connection
            connection.Open();


            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);




            cmd.Parameters.AddWithValue("@level_id", textBox1.Text);
            cmd.Parameters.AddWithValue("@subject_name", textBox2.Text.ToLower());


            //Execute command
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
