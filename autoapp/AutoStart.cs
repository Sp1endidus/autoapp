using System;
using System.Threading;
using System.Diagnostics;

namespace AutoApp
{
    class AutoStart
    {
        private string appPath;
        private string appName;
        private int delay;

        public AutoStart(string _app, int _delay)
        {
            appPath = _app;
            delay = _delay;
        }

        public void Start()
        {
            Log.L("Starting " + appPath);
            Process process = Process.Start(appPath);
            appName = process.ProcessName;
        }

        public void StartAuto()
        {
            try
            {
                while (true)
                {
                    if (!CheckProcesses())
                    {
                        Start();
                    }

                    Thread.Sleep(delay);
                }
            }
            catch (Exception e)
            {
                Log.L(e.ToString(), ConsoleColor.Red);
            }
        }

        private bool CheckProcesses()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(appName);
                if (processes.Length == 0)
                    return false;
            }
            catch (Exception e)
            {
                Log.L("Get processes error!", ConsoleColor.Red);
                Log.L(e.ToString(), ConsoleColor.Red);
            }
            return true;
        }
    }
}
