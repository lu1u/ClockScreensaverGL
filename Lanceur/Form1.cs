using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lanceur
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonParcourir_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = textBoxFileName.Text ;

            if ( openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxFileName.Text = openFileDialog.FileName;
                Settings.Default.FileName = openFileDialog.FileName;
                Settings.Default.Save();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.FileName = textBoxFileName.Text ;
            Settings.Default.Attendre = checkBoxAttendre.Checked;
            Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxFileName.Text = Settings.Default.FileName;
            checkBoxAttendre.Checked = Settings.Default.Attendre;
        }
    }
}
