﻿using MySql.Data.MySqlClient;
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
    public partial class DeleteFile : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public DeleteFile()
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                string query = "select subject_id from subjects where subject_name=@subject_name";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader;
                cmd.Parameters.AddWithValue("@subject_name", textBox1.Text.ToLower());
                dataReader = cmd.ExecuteReader();
                dataReader.Read();
                int subject_id = (int)dataReader["subject_id"];
                dataReader.Close();
                query = "delete from files where subject_id=@subject_id And file_name=@file_name";
                cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@subject_id", subject_id);
                cmd.Parameters.AddWithValue("@file_name", textBox2.Text.ToLower());
                cmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show(textBox2.Text + " Deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connection.Close();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
