using RosalESProfilingSystem.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RosalESProfilingSystem
{
    static class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetProcessDPIAware(); // Prevents scaling
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SplashScreen splash = new SplashScreen();
            splash.Show();
            Application.DoEvents();

            System.Threading.Thread.Sleep(3000);
            splash.Close();
            Application.Run(new Forms.Login_Form());
        }
    }
}
