Starts arbitrary program automatically.

USAGE:

[Required]<br />
-app %app_path%

-appargs %app_arguments%<br />
-apptocheck %app_which_will_be_checked_for_running%<br />
-delay %time_in_ms%<br />
-logs %true_or_false%<br />
-vpn %path_to_vpn_program%<br />
-vpnargs %arguments_for_vpn_program%<br />
-checkaddr %host_ip_or_name_for_echo_request%<br />
-startdelay %delay_before_start_app_in_ms%

Or you can create config file and set only this argument:<br />
-config %path_to_config%

EXAMPLE:

-app notepad<br />
-app "C:\Program Files\Internet Explorer\iexplore.exe" -delay 3000<br />
-app D:\my_program.exe -delay 500 -logs true<br />
-app mstsc -vpn "C:\Program Files\OpenVPN\bin\openvpn-gui.exe" -vpnargs "--connect myconfig.ovpn" -checkaddr myinternalhost.mydomain.local

CONFIG EXAMPLE:<br />
app=explorer<br />
appargs=mstsc<br />
apptocheck=mstsc<br />
vpn=C:\Program Files\OpenVPN\bin\openvpn-gui.exe<br />
vpnargs=--connect myconfig.ovpn<br />
checkaddr=myinternalhost.mydomain.local