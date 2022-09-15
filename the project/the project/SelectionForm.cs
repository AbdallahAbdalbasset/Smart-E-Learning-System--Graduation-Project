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
    public partial class SelectionForm : Form
    {
        public SelectionForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            new AddingSubject().ShowDialog();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
           
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            new AddingFile().ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
            new DeleteSubject().ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Dispose();
            new DeleteFile().ShowDialog();
        }
    }
}
