using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using IronPdf;
using System.Globalization;
using System.IO;
using Google.Cloud.Speech.V1;
using NReco.VideoConverter;
using MySql.Data.MySqlClient;
using DarrenLee.SpeechSynthesis;

namespace the_project
{
    public partial class MainForm : Form
    {
        
        List<Level> levels = new List<Level>();
        SpeechRecognitionEngine listner = new SpeechRecognitionEngine();
        SpeechSynthesizer reader = new SpeechSynthesizer();
        PdfDocument PDF;
        List<string> subchoces = new List<string>();
        Choices c;
        int pagenum = 0;
        int currentsubject;
        int currentsection =0;
        int currentfile;
        int currentlevel;

        //database variable
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public MainForm()
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

     
        private void Form1_Load(object sender, EventArgs e)
        {


            // input data for test
            settestdata();
            

            //adding choices
            addsubjectsfilesmaincommands();

            //setting the SpeechRecognitionEngine
            c = new Choices(subchoces.ToArray());
            Grammar g = new Grammar(c);
            listner.LoadGrammarAsync(g);
            listner.SetInputToDefaultAudioDevice();
            listner.SpeechRecognized += S_SpeechRecognized;
            textBox1.KeyUp += TestForm_KeyUp;
            adjustchatbot();
            
           

            startchatbot();
        }
        
           
        private void S_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "page next":
                    
                    if (pagenum < PDF.PageCount)
                    {
                        pagenum++;
                        adjustchatbot();
                        reader.SpeakAsync(pagenum.ToString());
                        textBox1.Clear();
                        textBox1.Text = PDF.ExtractTextFromPage(pagenum - 1);
                    }
                    else
                    {
                        adjustchatbot();
                        reader.SpeakAsync("هذه اخر صفحة بالكتاب");
                    }
                        
                    break;
                case "page back":
                    
                    if (pagenum >0)
                    {
                        pagenum--;
                        adjustchatbot();
                        reader.SpeakAsync(pagenum.ToString());
                        textBox1.Clear();
                        textBox1.Text = PDF.ExtractTextFromPage(pagenum - 1);
                    }

                    else
                    {
                        adjustchatbot();
                        reader.SpeakAsync("هذه اول صفحة بالكتاب");
                    }
                        
                    break;
                case "Start reading":
                    adjustchatbot();
                    reader.SpeakAsync(textBox1.Text);
                    break; 
                case "Continue":
                    if (reader.State == SynthesizerState.Paused)
                    {
                        //Pauses the SpeechSynthesizer object.   
                        reader.Resume();

                    }
                    break;
                case "Close the program":
                    Application.Exit();
                    break;
                case "List markers":
                    adjustchatbot();
                    foreach (var tt in PDF.BookMarks.BookMarkList)
                    {
                        reader.SpeakAsync(tt.Text);
                    }
                    
                    break;
                case "section":

                    adjustchatbot();
                    if (currentsection < PDF.BookMarks.BookMarkList.Count)
                    {
                        pagenum = PDF.BookMarks.BookMarkList[currentsection].PageIndex + 1;
                        textBox1.Text = PDF.ExtractTextFromPage(pagenum);
                        reader.Speak("لقد تم اختيار فصل رقم" + " " + (currentsection + 1).ToString());
                        reader.SpeakAsync("رقم الصفحة " + pagenum.ToString());
                        currentsection++;
                        currentsection = currentsection % (PDF.BookMarks.BookMarkList.Count-1);
                    }     
                    else
                    {
                        adjustchatbot();
                        reader.SpeakAsync("هذا اخر فصل بالملف");
                    }
                        
                    break;

                case "List subject files":
                    pagenum = 0;
                    currentsection = 0;
                    readsubjectfiles();
                    break;
                case "list level materials":
                    pagenum = 0;
                    currentsection = 0;
                    readlevelsubjects();
                    break;
                case "list Levels":
                    pagenum = 0;
                    currentsection = 0;
                    startchatbot();
                    break;

            }

            int i = -1;
            foreach(var l in levels)
            {
                i++;
                if(l.levelname == e.Result.Text)
                {
                    currentlevel = i;
                    adjustchatbot();
                    reader.Speak("لقد تم اختيار المستوى" + " " + (currentlevel+1));
                    readlevelsubjects();
                    return;

                }
            }
            i = 0;
            foreach (var t in levels[currentlevel].subjectsnames)
            {
                if (t.name == e.Result.Text)
                {
                    adjustchatbot();
                    reader.Speak("لقد تم اختيار مادة"+" "+t.name);
                    currentsubject = i;
                    readsubjectfiles();
                    return;
                }
                i++;
            }
            i = -1;
            foreach (var x in levels[currentlevel].subjectsnames[currentsubject].books)
            {
                i++;
                if (e.Result.Text == x)
                {
                    currentfile = i;
                    adjustchatbot();
                    reader.SpeakAsync("لقة تم اختيار ملف" +" "+x );
                    

                    retriev(levels[currentlevel].subjectsnames[currentsubject].name, x);
                    
                    PDF = PdfDocument.FromFile("Used.pdf");
                    reader.SpeakAsync("هذا الملف به" + PDF.PageCount + "من الصفحات");
                    textBox1.Clear();
                    pagenum = 0;
                    textBox1.Text = PDF.ExtractTextFromPage(pagenum);
                    Listmarkers();
                    addbookmarkersascommands();
                                
                }
               
            }
            if (PDF != null)
            {
                int m = 0;
                foreach (var tt in PDF.BookMarks.BookMarkList)
                {
                    m++;
                    if (e.Result.Text == tt.Text)
                    {
                        adjustchatbot();
                        pagenum = tt.PageIndex + 1;
                        
                        reader.Speak("لقد تم اختيار فصل رقم" + " " + (m).ToString());
                        reader.SpeakAsync("رقم الصفحة "+pagenum.ToString());
                        textBox1.Clear();
                        textBox1.Text = PDF.ExtractTextFromPage(pagenum - 1);
                    }
                }
            }
        }
        
        void readsubjectfiles()
        {

            listBox1.Items.Clear();
            foreach (var x in levels[currentlevel].subjectsnames[currentsubject].books)
            {
               listBox1.Items.Add(x);
            }
            adjustchatbot();
            reader.SpeakAsync("هذه المادة تحتوي على الملفات التالية");
            int counter = 1;
            foreach (var x in levels[currentlevel].subjectsnames[currentsubject].books)
            {
                
                reader.SpeakAsync("الملف رقم " + counter.ToString());
                reader.SpeakAsync(x);
                counter++;
            }
        }

        void readlevelsubjects()
        {

            listBox1.Items.Clear();
            foreach (var s in levels[currentlevel].subjectsnames)
            {
                listBox1.Items.Add(s.name);
            }
            adjustchatbot();
            reader.SpeakAsync("هذا المستوى يحتوي على المواد التالية");
            int counter = 1;  
            foreach (var s in levels[currentlevel].subjectsnames)
            {
                reader.SpeakAsync("المادة رقم " + counter.ToString());
                reader.SpeakAsync(s.name);
                counter++;
            }
        }

        void Listmarkers()
        {
            listBox1.Items.Clear();
            foreach(var t in PDF.BookMarks.BookMarkList)
            {
                listBox1.Items.Add(t.Text);
            }                 
        }

        void addsubjectsfilesmaincommands()
        {
            //adding the commands
            subchoces.Add("page next");
            subchoces.Add("page back");
            subchoces.Add("Start reading");
            subchoces.Add("Continue");
            subchoces.Add("Close the program");
            subchoces.Add("List subject files");
            subchoces.Add("List markers");
            subchoces.Add("section");
            subchoces.Add("list level materials"); 
            subchoces.Add("list Levels");
           


            //adding the database data
            foreach (var l in levels)
            {
                subchoces.Add(l.levelname);
                foreach(var s in l.subjectsnames)
                {
                    subchoces.Add(s.name);
                    foreach(var n in s.books)
                    {
                        subchoces.Add(n);
                    }
                }
            }
        }

        void startchatbot()
        {

            listBox1.Items.Clear();
            foreach (var l in levels)
            {
                listBox1.Items.Add(l.levelname);
            }
            adjustchatbot();

            reader.SpeakAsync("السلام عليكم");
            reader.SpeakAsync("يرجي اختيار المستوى");
            int number = 1;
            foreach (var l in levels)
            {
                if (reader.State != SynthesizerState.Paused)
                {
                    reader.SpeakAsync("المستوى رقم" + number.ToString());
                    number++;
                }

            }

        }

        void settestdata()
        {
            for (int i = 1; i <= 4; i++)
            {
                levels.Add(new Level(i));
            }
            levels[0].levelname = "One";
            levels[1].levelname = "Two";
            levels[2].levelname = "Three";
            levels[3].levelname = "Four";

        }
        private void TestForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                reader.Pause();
                if(listner.AudioState == AudioState.Stopped)
                {
                    listner.RecognizeAsync(RecognizeMode.Single);
                }
                
            }
        }
        void addbookmarkersascommands()
        {
            subchoces.Clear();
            addsubjectsfilesmaincommands();
            listner.UnloadAllGrammars();
            foreach(var name in PDF.BookMarks.BookMarkList)
            {
                subchoces.Add(name.Text);
            }
            c = new Choices(subchoces.ToArray());
            Grammar g = new Grammar(c);
            listner.LoadGrammarAsync(g);
        }
        void retriev(string subject_name, string pdf)
        {
            int subject_id;
            string query1 = "select subject_id from subjects where subject_name=@subject_name";
            byte[] data;
            //Open connection
            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query1, connection);
                cmd.Parameters.AddWithValue("@subject_name", subject_name);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                dataReader.Read();
                subject_id = dataReader.GetInt32(0);
                dataReader.Close();
                query1 = "select file_content from files where subject_id=@subject_id AND file_name=@file_name";
                //Create Command
                cmd = new MySqlCommand(query1, connection);
                //Create a data reader and Execute the command



                cmd.Parameters.AddWithValue("@subject_id", subject_id);
                cmd.Parameters.AddWithValue("@file_name", pdf);
                 dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list
                while (dataReader.Read())
                {

                    data = (byte[])dataReader["file_content"];
                    if(File.Exists("Used.pdf"))
                    {
                        File.Delete("Used.pdf");
                    }
                    using (FileStream fs = new FileStream("Used.pdf", FileMode.Create))
                    {
                        fs.Write(data, 0, data.Length);
                    }


                }


                //close Data Reader
                dataReader.Close();

                //close Connection
                CloseConnection();

            }
        }

            bool OpenConnection()
            {
                try
                {
                    this.connection.Open();
                    return true;
                }
                catch (MySqlException ex)
                {
                    //When handling errors, you can your application's response based 
                    //on the error number.
                    //The two most common error numbers when connecting are as follows:
                    //0: Cannot connect to server.
                    //1045: Invalid user name and/or password.
                    switch (ex.Number)
                    {
                        case 0:
                            MessageBox.Show("Cannot connect to server.  Contact administrator");
                            break;

                        case 1045:
                            MessageBox.Show("Invalid username/password, please try again");
                            break;
                    }
                    return false;
                }

            }

            //Close connection
            bool CloseConnection()
            {
                try
                {
                    this.connection.Close();
                    return true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            reader.Pause();
            
            new Login().ShowDialog();
        }
        void adjustchatbot()
        {
            reader.Resume();
            reader.Dispose();
            reader = new SpeechSynthesizer();
            reader.SelectVoice("Microsoft Hoda");
            reader.Rate = -2;

        }
    }
    }
    
    

    

