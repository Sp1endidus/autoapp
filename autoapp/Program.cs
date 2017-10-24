using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace AutoApp
{
    class Program
    {
        //Local
        private static int falseCount = 0;
        private static int falseLimit = 256;

        //Arguments
        private static ArgsReader AppToStart = new ArgsReader("app", "");
        private static ArgsReader Delay = new ArgsReader("delay", 2000);
        private static ArgsReader LogsOn = new ArgsReader("logs", false);

        //Hide console
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;


        static void Main(string[] args)
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            try
            {
                AppToStart.ReadArgs(args, typeof(string));
                Delay.ReadArgs(args, typeof(int));
                LogsOn.ReadArgs(args, typeof(bool));
            }
            catch (Exception e)
            {
                ShowWindow(handle, SW_SHOW);
                Log.L("Read arguments error!", ConsoleColor.Red);
                Log.L(e.ToString(), ConsoleColor.Red);
                Console.ReadLine();
                return;
            }

            if (Convert.ToBoolean(LogsOn.Value) == true)
            {
                ShowWindow(handle, SW_SHOW);
            }

            if ((string)AppToStart.Value == "")
            {
                ShowWindow(handle, SW_SHOW);
                Log.L("Enter app to start (-app notepad)", ConsoleColor.Red);
                Console.ReadLine();
                return;
            }

            AutoStart autoStart;

            try
            {
                autoStart = new AutoStart((string)AppToStart.Value, Convert.ToInt32(Delay.Value));
            }
            catch (Exception e)
            {
                ShowWindow(handle, SW_SHOW);
                Log.L("Autostart error!", ConsoleColor.Red);
                Log.L(e.ToString(), ConsoleColor.Red);
                Console.ReadLine();
                return;
            }

            while (true)
            {
                try
                {
                    autoStart.StartAuto();
                }
                catch (Exception e)
                {
                    Log.L(e.ToString(), ConsoleColor.Red);
                    if (falseCount >= falseLimit)
                        return;
                }

                try
                {
                    Thread.Sleep(Convert.ToInt32(Delay.Value));
                }
                catch (Exception e)
                {
                    Log.L(e.ToString(), ConsoleColor.Red);
                    falseCount++;
                    if (falseCount >= falseLimit)
                        return;
                }
            }
        }
    }
}
