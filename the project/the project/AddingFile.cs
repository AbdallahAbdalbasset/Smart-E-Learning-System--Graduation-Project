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
using System.IO;
using System.Threading;

namespace the_project
{
    public partial class AddingFile : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        string filename;
        public AddingFile()
        {
            InitializeComponent();
            openFileDialog1.Title = "Select a PDF file";
            openFileDialog1.FileName = "";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "PDF files|*.pdf|All files|*.*";
            server = "mysql-81913-0.cloudclusters.net";
            database = "Faculty";
            uid = "admin";
            password = "1SOin0zW";
            string connectionString;
            connectionString = "SERVER=" + server + "; PORT = 19902 ;" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread uploadfile = new Thread(new ThreadStart(upload));
            uploadfile.Start();
           

        }
        void upload()
        {
            try
            {
                connection.Open();
                int subject_id;
                string query1 = "select subject_id from subjects where subject_name=@subject_name";
                string query = "insert into files (subject_id,file_name,file_content) Values (@subject_id,@file_name,@file_content)";
                MySqlCommand cmd = new MySqlCommand(query1, connection);
                cmd.Parameters.AddWithValue("@subject_name", textBox1.Text.ToLower());
                MySqlDataReader dataReader = cmd.ExecuteReader();
                dataReader.Read();
                subject_id = dataReader.GetInt32(0);
                dataReader.Close();

                //open connection


                cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@subject_id", subject_id);
                cmd.Parameters.AddWithValue("@file_name", Path.GetFileName(filename).Remove(Path.GetFileName(filename).Length - 4, 4));
                byte[] file = File.ReadAllBytes(openFileDialog1.FileName);
                cmd.Parameters.Add("@file_content", MySqlDbType.VarBinary).Value = file;

                cmd.CommandTimeout = int.MaxValue;
                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                connection.Close();
                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName.ToLower();
            }
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }
    }
}
