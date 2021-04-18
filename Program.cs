using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rotrGRP
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Template();
            Application.Run(new Form1());
        }

        static void Template()
        {
            using (StreamWriter sw = File.CreateText(".\\settings"))
            {
                sw.WriteLine("textures\\block\\\noptifine\\ctm\\");
            }
        }
    }
}
