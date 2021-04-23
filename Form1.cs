using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Newtonsoft.Json.Linq;
using Syncfusion.Windows.Forms.Tools;

namespace rotrGRP
{

    public partial class Form1 : Form
    {
        public Point mouseLocation;
        string[] save;
        public Form1()
        {
            InitializeComponent();
            if (File.Exists(".\\save"))
            {
                save = File.ReadAllLines(".\\save");    
                RName.Text = save[0];
                RDesc.Text = save[1];
                if (save[2] == "Active")
                {
                    toggle16.ToggleState = ToggleButtonState.Active;
                }
                if (save[3] == "Active")
                {
                    toggle15.ToggleState = ToggleButtonState.Active;
                }
                if (save[4] == "Active")
                {
                    toggle14.ToggleState = ToggleButtonState.Active;
                }
                if (save[5] == "Active")
                {
                    toggle12.ToggleState = ToggleButtonState.Active;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

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

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        bool m16_2048 = true;
        bool m16_1024 = true;
        bool m16_512 = true;
        bool m16_256 = true;


        string path;
        float fileProg = 0.0F;
        int versions;
        string[] settings;
        public delegate void InvokeDelegate();
        public delegate void InvokeDelegateMeta(string path, int version);
        public delegate void InvokeDelegateLog(string text);
        Form2 settingsForm;
        Boolean Started = false;
        Task Program;
        void PreStart()
        {
            if (RName.Text != "")
            {
                save = new string[6];
                save[0] = RName.Text; save[1] = RDesc.Text;
                save[2] = toggle16.ToggleState.ToString();
                save[3] = toggle15.ToggleState.ToString();
                save[4] = toggle14.ToggleState.ToString();
                save[5] = toggle12.ToggleState.ToString();
                using (StreamWriter sw = File.CreateText(".\\save"))
                {
                    sw.WriteLine(String.Join("\n", save));
                }
                Logs("Load settings");
                if (File.Exists(".\\settings"))
                {
                    settings = File.ReadAllLines(".\\settings");
                }
                else
                {
                    settings[0] = "textures\\block\\"; settings[1] = "optifine\\ctm\\";
                    using (StreamWriter sw = File.CreateText(".\\settings"))
                    {
                        sw.WriteLine(String.Join("\n", settings));
                    }
                }
                path = ".\\" + RName.Text + " ResourcePacks\\";
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                fileBar.Value = 0;
                versions = 0;
                fileBar.Visible = true;
                Program = new Task(() => Start());
                Program.Start();
            }
        }
        private void button17_Click(object sender, EventArgs e)
        {
            if (!Started)
            {
                Started = true;
                button17.Text = "Stop";
                PreStart();
            }
            else
            {
                Started = false;
                Application.Restart();
            }
        }
        void Start()
        {
            Logs("Start..");
            if (toggle16.ToggleState.ToString() == "Active")
            {
                versions = versions + 1;
            }
            if (toggle15.ToggleState.ToString() == "Active")
            {
                versions = versions + 1;
            }
            if (toggle14.ToggleState.ToString() == "Active")
            {
                versions = versions + 1;
            }
            if (toggle12.ToggleState.ToString() == "Active")
            {
                versions = versions + 1;
            }
            if (toggle16.ToggleState.ToString() == "Active")
            {
                GenerateV(6);
            }
            if (toggle15.ToggleState.ToString() == "Active")
            {
                GenerateV(5);
            }
            if (toggle14.ToggleState.ToString() == "Active")
            {
                GenerateV(4);
            }
            if (toggle12.ToggleState.ToString() == "Active")
            {
                GenerateV(2);
            }
            Logs("Done");
        }

        void GenerateV(int version)
        {
            Logs("Preparing the version 1." + version);
            if (m16_2048) { GenerateT(version, 2048); }
            if (m16_1024) { GenerateT(version, 1024); }
            if (m16_512) { GenerateT(version, 512); }
            if (m16_256) { GenerateT(version, 256); }
        }

        void GenerateT(int version, int type)
        {
            string tpath;
            string fpath;
            if (version != 4)
            {
                Logs("Preparing a resourcepack x" + type + " for 1.1" + version);
                tpath = path + RName.Text + " 1.1" + version + " x" + type;
                fpath = tpath + "\\assets\\minecraft\\";
                Logs("Create folder");
                Directory.CreateDirectory(tpath);
            }
            else
            {
                Logs("Preparing a resourcepack x" + type + " for  1.14-13");
                tpath = path + RName.Text + " 1.14-13 x" + type;
                fpath = tpath + "\\assets\\minecraft\\";
                Logs("Create folder");
                Directory.CreateDirectory(tpath);
            }

            GenerateP(tpath, version, type);
            switch (type)
            {
                case 2048:
                    break;
                case 1024:
                    Resizing(fpath, 2);
                    break;
                case 512:
                    Resizing(fpath, 4);
                    break;
                case 256:
                    Resizing(fpath, 8);
                    break;
            }
            Logs("Archiving a resource pack..");
            BeginInvoke(new InvokeDelegate(UpdateBar));
            ZipFile.CreateFromDirectory(tpath, tpath + ".zip");
            //Directory.Delete(tpath, true);
        }

        void Resizing(string path, int scale)
        {
            Logs("Generation path");
            int i = 1;
            string[] Textures;
            Image image;
            Bitmap bitmap;
            Textures = Directory.GetFiles(path + settings[0], "*", SearchOption.AllDirectories);
            while (i < settings.Length)
            {
                Logs("Read " + settings.Length + " settings lines..");
                Textures = Textures.Concat(Directory.GetFiles(path + settings[i], "*", SearchOption.AllDirectories)).ToArray();
                Console.WriteLine(Textures.Length);
                i++;
            }
            i = 0;
            while (i < Textures.Length)
            {
                if (Path.GetFileName(Textures[i]).EndsWith(".png"))
                {
                    Logs("Resize " + Path.GetFileName(Textures[i]));
                    image = Image.FromFile(Textures[i]);
                    bitmap = new Bitmap(image, new Size(2048 / scale, 2048 / scale));
                    Logs("Despose texture " + Path.GetFileName(Textures[i]));
                    image.Dispose();
                    File.Delete(Textures[i]);
                    Logs("Save texture " + Path.GetFileName(Textures[i]));
                    bitmap.Save(Textures[i], System.Drawing.Imaging.ImageFormat.Png);
                }
                i++;
            }
        }

        void GenerateP(string tpath, int version, int type)
        {
            Logs("Create assets folder x" + type + " for 1.1" + version);
            Directory.CreateDirectory(tpath + "\\assets");
            string dpath = ".\\DEV";
            string path = tpath;
            Logs("Copy icon..");
            File.Copy(dpath + "\\pack.png", path + "\\pack.png");
            Logs("Create mcmeta..");
            BeginInvoke(new InvokeDelegateMeta(Desc), path, version);
            Logs("Copying original files x" + type + " for 1.1" + version);
            if (version == 2)
            {
                RenameCopy(dpath + "\\assets", path + "\\assets");
            }
            else
            {
                //CopyFolder(dpath + "\\assets", path + "\\assets");
                RenameCopy(dpath + "\\assets", path + "\\assets");
            }

            Logs("Copy done");
        }
        void RenameCopy(string sourceFolder, string destFolder)
        {
            string jsonblocks12 = File.ReadAllText("D:/ROTR/GitHub/rotrGRP/Resources/blocks.json");
            JObject blocks12 = JObject.Parse(jsonblocks12);

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string nameex = Path.GetFileNameWithoutExtension(file);
                string ex = name.Replace(nameex, "");
                string dest = "";
                //nameex = Path.GetFileName(file);
                //name = Path.GetFileNameWithoutExtension(file);
                // ex = nameex.Replace(nameex, name);
                //dest = destFolder + name + ex;
                //dest = Path.Combine(destFolder, blocks12.SelectToken(name).ToString());
                //dest = dest + ex;
                if (nameex.EndsWith("_n"))
                {
                    nameex = name.Replace("_n.png", "");
                    try
                    {
                        dest = Path.Combine(destFolder, blocks12.SelectToken(nameex).ToString());
                    }
                    catch (Exception)
                    {
                        dest = Path.Combine(destFolder, nameex);
                    }
                    dest = Path.Combine(dest + "_n.png");
                }
                else
                {
                    if (nameex.EndsWith("_s"))
                    {
                        nameex = name.Replace("_s.png", "");
                        try
                        {
                            dest = Path.Combine(destFolder, blocks12.SelectToken(nameex).ToString());
                        }
                        catch (Exception)
                        {
                            dest = Path.Combine(destFolder, nameex);
                        }
                        dest = Path.Combine(dest + "_s.png");
                    }
                    else
                    {
                        try
                        {
                            dest = Path.Combine(destFolder, blocks12.SelectToken(nameex).ToString());
                        }
                        catch (Exception)
                        {
                            dest = Path.Combine(destFolder, nameex);
                        }
                        dest = Path.Combine(dest + ex);
                    }
                }
                File.Copy(file, dest);
                Logs("Copying " + Path.GetFileName(file));
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                RenameCopy(folder, dest);
            }
        }
        void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                Logs("Copying " + Path.GetFileName(file));
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }
        void Logs(string text)
        {
            BeginInvoke(new InvokeDelegateLog(SetLogs), text);
        }
        public void SetLogs(string text)
        {
            textLog.Text = text;
        }
        public void UpdateBar()
        {
            fileProg = fileProg + 100 / (versions * 4);
            fileBar.Value = (int)fileProg;
        }
        public void Desc(string path, int version)
        {
            using (StreamWriter sw = File.CreateText(path + "\\pack.mcmeta"))
            {
                if (version==2)
                {
                    sw.WriteLine("{\n\t\"pack\": {\n\t\t\"pack_format\": " + (version+1) + ",\n\t\t\"description\": \"" + RDesc.Text + "\"\n\t}\n}");
                }
                else
                {
                    sw.WriteLine("{\n\t\"pack\": {\n\t\t\"pack_format\": " + version + ",\n\t\t\"description\": \"" + RDesc.Text + "\"\n\t}\n}");
                }
                
            }
        }
        private void button18_Click(object sender, EventArgs e)
        {
           if (settingsForm == null)
            {
                settingsForm = new Form2();
                settingsForm.Show();
            }
            else
            {
                if (settingsForm.Visible == true)
                {
                    settingsForm.Focus();
                }
                else
                {
                    settingsForm.Show();
                }
            }
        }
        void ChangeBC(int type, Color color)
        {
            switch (type)
            {
                case 2048:
                    button4.ForeColor = color;
                    break;
                case 1024:
                    button3.ForeColor = color;
                    break;
                case 512:
                    button2.ForeColor = color;
                    break;
                case 256:
                    button1.ForeColor = color;
                    break;
            }
        }
        //2048
        private void button4_Click(object sender, EventArgs e)
        {
            if (m16_2048)
            {
                m16_2048 = false;
                ChangeBC(2048, Color.DimGray);
            }
            else
            {
                m16_2048 = true;
                ChangeBC(2048, Color.Gainsboro);
            }
        }
        //1024

        private void button3_Click(object sender, EventArgs e)
        {
            if (m16_1024)
            {
                m16_1024 = false;
                ChangeBC(1024, Color.DimGray);
            }
            else
            {
                m16_1024 = true;
                ChangeBC(1024, Color.Gainsboro);
            }
        }
        //512
        private void button2_Click(object sender, EventArgs e)
        {
            if (m16_512)
            {
                m16_512 = false;
                ChangeBC(512, Color.DimGray);
            }
            else
            {
                m16_512 = true;
                ChangeBC(512, Color.Gainsboro);
            }
        }
        //256
        private void button1_Click(object sender, EventArgs e)
        {
            if (m16_256)
            {
                m16_256 = false;
                ChangeBC(256, Color.DimGray);
            }
            else
            {
                m16_256 = true;
                ChangeBC(256, Color.Gainsboro);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (RName.Text != "")
            {
                path = ".\\" + RName.Text + " ResourcePacks\\";
                if (Directory.Exists(path))
                {
                    Logs("Open " + path);
                    System.Diagnostics.Process.Start(path);

                }
                else
                {
                    Logs("The folder is empty, start generation");
                }
            }
            else
            {
                Logs("Name field is empty");
            }
        }
    }
}