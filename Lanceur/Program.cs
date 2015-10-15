using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lanceur
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower().Trim().Substring(0, 2) == "/c") //configure
                {
                    Application.Run(new Form1());
                }
                else
                    ShowScreensaver(args);
                    
            }
            else //no arguments were passed
            {
                Application.Run(new Form1());
            }
        }

        private static void ShowScreensaver(string[] args)
        {
            if (Settings.Default.FileName.Length == 0 || !File.Exists(Settings.Default.FileName))
            {
                Application.Run(new Form1());
                return;
            }
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = Settings.Default.FileName;
                if (args != null)
                {
                    for (int i = 0; i < args.Length; i++)
                        p.StartInfo.Arguments += args[i] + " ";
                }

                p.Start();

                if (Settings.Default.Attendre)
                    p.WaitForExit();
            }
            catch( Exception e)
            {
                MessageBox.Show(e.Source + "\nFilename:" + Settings.Default.FileName, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
