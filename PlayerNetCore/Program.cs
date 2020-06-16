using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PlayerNetCore
{
    public static class Program
    {

        [STAThread]
        public static int Main(string[] args)
        {
            App application = new App();
            application.InitializeComponent();
            application.Run();
            bool requireExit = false;
            application.Exit += (sender, e) => requireExit = true;
            while (!requireExit)
            {

            }
            return 0;
        }
    }
}
