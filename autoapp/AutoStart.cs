using System;
using System.Threading;
using System.Diagnostics;

namespace AutoApp
{
    class AutoStart
    {
        private string appPath;
        //private string appName;
        private string appArgs;
        private string appToCheck;
        private int delay;

        public AutoStart(string _app, string _appArgs, int _delay, string _appToCheck)
        {
            appPath = _app;
            appArgs = _appArgs;
            delay = _delay;
            appToCheck = _appToCheck;
        }

        public AutoStart(string _app, string _appArgs, int _delay) : this(_app, _appArgs, _delay, "")
        { }

        public void Start()
        {
            Log.L("Starting " + appPath + " with args " + appArgs);
            Process process = Process.Start(appPath, appArgs);
            if (appToCheck == "") { appToCheck = process.ProcessName; }
            //appName = process.ProcessName;
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
                Process[] processes = Process.GetProcessesByName(appToCheck);
                Log.L("Checking " + appToCheck + " count - " + processes.Length.ToString());
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
