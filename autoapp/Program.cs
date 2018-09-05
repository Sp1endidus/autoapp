using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace AutoApp
{
    class Program
    {
        //Local
        private static int falseCount = 0;
        private static int falseLimit = 256;

        //Arguments
        private static ArgsReader ConfigPath = new ArgsReader("config", "autoapp_config.txt");
        private static ArgsReader AppToStart = new ArgsReader("app", "");
        private static ArgsReader AppArgs = new ArgsReader("appargs", "");
        private static ArgsReader AppToCheck = new ArgsReader("apptocheck", "");
        private static ArgsReader Delay = new ArgsReader("delay", 2000);
        private static ArgsReader LogsOn = new ArgsReader("logs", false);
        private static ArgsReader VpnPath = new ArgsReader("vpn", "");
        private static ArgsReader VpnArgs = new ArgsReader("vpnargs", "");
        private static ArgsReader CheckConnection = new ArgsReader("checkaddr", "");
        private static ArgsReader StartDelay = new ArgsReader("startdelay", 0);

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
                ConfigPath.ReadArgs(args, typeof(string));
                AppToStart.ReadArgs(args, typeof(string));
                AppArgs.ReadArgs(args, typeof(string));
                AppToCheck.ReadArgs(args, typeof(string));
                Delay.ReadArgs(args, typeof(int));
                LogsOn.ReadArgs(args, typeof(bool));
                VpnPath.ReadArgs(args, typeof(string));
                VpnArgs.ReadArgs(args, typeof(string));
                CheckConnection.ReadArgs(args, typeof(string));
                StartDelay.ReadArgs(args, typeof(int));
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

            //Loading config
            try
            {
                Config config = new Config((string)ConfigPath.Value, true);
                if (config.ConfigLoadedSuccessful)
                {
                    if (config.Values.ContainsKey(AppToStart.Argument)) { AppToStart.Value = config.GetValue<object>(AppToStart.Argument); }
                    if (config.Values.ContainsKey(AppArgs.Argument)) { AppArgs.Value = config.GetValue<object>(AppArgs.Argument); }
                    if (config.Values.ContainsKey(AppToCheck.Argument)) { AppToCheck.Value = config.GetValue<object>(AppToCheck.Argument); }
                    if (config.Values.ContainsKey(Delay.Argument)) { Delay.Value = config.GetValue<object>(Delay.Argument); }
                    if (config.Values.ContainsKey(LogsOn.Argument)) { LogsOn.Value = config.GetValue<object>(LogsOn.Argument); }
                    if (config.Values.ContainsKey(VpnPath.Argument)) { VpnPath.Value = config.GetValue<object>(VpnPath.Argument); }
                    if (config.Values.ContainsKey(VpnArgs.Argument)) { VpnArgs.Value = config.GetValue<object>(VpnArgs.Argument); }
                    if (config.Values.ContainsKey(CheckConnection.Argument)) { CheckConnection.Value = config.GetValue<object>(CheckConnection.Argument); }
                    if (config.Values.ContainsKey(StartDelay.Argument)) { StartDelay.Value = config.GetValue<object>(StartDelay.Argument); }
                }
            }
            catch (Exception e)
            {
                Log.L("Config error!\r\n" + e.ToString());
            }

            //App set requred
            if ((string)AppToStart.Value == "")
            {
                ShowWindow(handle, SW_SHOW);
                Log.L(HelpMessage.Help, ConsoleColor.White);
                Console.ReadLine();
                return;
            }

            //Check connection before connecting VPN
            bool needVPN = true;
            if ((string)CheckConnection.Value != "")
            {
                Ping ping = new Ping();
                try
                {
                    PingReply pingReply = ping.Send((string)CheckConnection.Value);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        needVPN = false;
                    }
                }
                catch { }
            }

            //Start VPN if it has and if it needs
            bool vpnStarted = false;
            if ((string)VpnPath.Value != "" && needVPN)
            {
                if (System.IO.File.Exists((string)VpnPath.Value))
                {
                    try
                    {
                        System.Diagnostics.Process.Start((string)VpnPath.Value, (string)VpnArgs.Value);
                        vpnStarted = true;
                    }
                    catch (Exception e)
                    {
                        ShowWindow(handle, SW_SHOW);
                        Log.L("VPN error!", ConsoleColor.Red);
                        Log.L(e.ToString(), ConsoleColor.Red);
                        Console.ReadLine();
                        return;
                    }
                }
            }

            //Check connection
            if ((string)CheckConnection.Value != "" && needVPN && vpnStarted)
            {
                Ping ping = new Ping();
                bool success = false;
                while (!success)
                {
                    try
                    {
                        PingReply pingReply = ping.Send((string)CheckConnection.Value);
                        if (pingReply.Status == IPStatus.Success)
                        {
                            success = true;
                        }

                        Console.WriteLine(pingReply.Status);
                    }
                    catch { }
                    if (!success)
                        Thread.Sleep(1000);
                }
            }

            //AutoStart
            AutoStart autoStart;

            try
            {
                autoStart = new AutoStart((string)AppToStart.Value, (string)AppArgs.Value, Convert.ToInt32(Delay.Value), (string)AppToCheck.Value);
            }
            catch (Exception e)
            {
                ShowWindow(handle, SW_SHOW);
                Log.L("Autostart error!", ConsoleColor.Red);
                Log.L(e.ToString(), ConsoleColor.Red);
                Console.ReadLine();
                return;
            }

            if (Convert.ToInt32(StartDelay.Value) > 0)
            {
                try
                {
                    Thread.Sleep(Convert.ToInt32(StartDelay.Value));
                }
                catch (Exception e)
                {
                    Log.L(e.ToString(), ConsoleColor.Red);
                    falseCount++;
                }
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

    class HelpMessage
    {
        public static readonly string Help =
@"USAGE:

[Required]
-app %app_path%

-appargs %app_arguments%
-apptocheck % app_which_will_be_checked_for_running%
-delay %time_in_ms%
-logs %true_or_false%
-vpn %path_to_vpn_program%
-vpnargs %arguments_for_vpn_program%
-checkaddr %host_ip_or_name_for_echo_request%
-startdelay %delay_before_start_app_in_ms%

Or you can create config file and set only this argument:
-config %path_to_config%

EXAMPLE:

-app notepad
-app ""C:\Program Files\Internet Explorer\iexplore.exe"" -delay 3000
-app D:\my_program.exe -delay 500 -logs true
-app mstsc -vpn ""C:\Program Files\OpenVPN\bin\openvpn-gui.exe"" -vpnargs ""--connect myconfig.ovpn"" -checkaddr myinternalhost.mydomain.local

CONFIG EXAMPLE:
app= explorer
appargs= mstsc
apptocheck= mstsc
vpn= C:\Program Files\OpenVPN\bin\openvpn-gui.exe
vpnargs = --connect myconfig.ovpn
checkaddr = myinternalhost.mydomain.local";
    }
}
