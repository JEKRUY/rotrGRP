using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rotrGRP
{
    public partial class Form2 : Form
    {
        public Point mouseLocation;
        string[] settings;
        public Form2()
        {
            InitializeComponent();
            settings = File.ReadAllLines(".\\settings");
            settingsBox.Text = String.Join("\n", settings);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
             mouseLocation = new Point(-e.X, -e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            settings = settingsBox.Lines.ToArray();
            using (StreamWriter sw = File.CreateText(".\\settings"))
            {
                sw.WriteLine(String.Join("\n", settings));
            }
            this.Hide();
        }
    }
}
