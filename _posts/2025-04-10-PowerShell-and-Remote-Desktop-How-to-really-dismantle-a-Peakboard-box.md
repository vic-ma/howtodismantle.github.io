---
layout: post
title: PowerShell and Remote Desktop Connection - How to really dismantle a Peakboard box
date: 2023-03-01 02:00:00 +0200
tags: administration
image: /assets/2025-04-10/title.png
image_header: /assets/2025-04-10/title_landscape.png
bg_alternative: true
read_more_links:
  - name: More articles around administration topics
    url: /category/administration
---
The Peakboard Box is designed to be functional as soon as the customer takes it out of the box and connects it to power and a network. The Peakboard runtime is designed to work perfectly and securely on the Peakboard Box's hardware. All non-essential software components are switched off or put to hibernation. This reduces the attack surface.

However, it's sometimes necessary for administrators to manage Peakboard boxes on an administrative level. This is possible. The Peakboard Box runs on Windows, and the credentials for the Windows system are given to the customer during the physical delivery.

Here are some examples of things you can do with administrative priviledges:

- Install Windows updates
- Install additional software to align the computer with internal security compliance guidelines
- Install additional software for an application that runs on the Peakboard runtime (e.g. ODBC drivers)
- Track and monitor error events in the event log
- Manually install Peakboard runtime updates
- Bring the device into the AD domain

Before we step into the technical details, you must understand that accessing the Peakboard Box with administrative-level credentials might pose a security risk, and it might reduce the stability of the system. So you should only engage in these activities when necessary.

## Log in with PowerShell

Follow these steps to log into a Peakboard Box's system, with PowerShell. Make sure to run PowerShell as administrator.

![image](/assets/2025-04-10/010.png)

1. The service `winrm` will be needed to manage remote administration task on other machines
2. The IP address of the box but must be white-listed
3. We can enter a remote session in the box through the IP. The adminstrator's password must be provided in a separate credential prompt.

{% highlight powershell %}
net start winrm
Set-Item WSMan:\localhost\Client\TrustedHosts -Value {IP}
Enter-PSSession -ComputerName {IP} -Credential {Box Hostname}\pbadmin
{% endhighlight %}

![image](/assets/2025-04-10/020.png)

![image](/assets/2025-04-10/025.png)

After having successfully established the PS session, we must change a value in the registry of the box to enable remote desktop connections. By default it's disabled. We also need to open the port for remote desktop in the firewall. Then we can exit the remote session and close the PS console.

{% highlight powershell %}
Set-ItemProperty -Path 'HKLM:\System\CurrentControlSet\Control\Terminal Server' -name "fDenyTSConnections" -value 0
Enable-NetFirewallRule -DisplayGroup "Remote Desktop"
exit
exit
{% endhighlight %}

![image](/assets/2025-04-10/030.png)

## Logon via Remote Desktop Connection

Now all the preparations are done to use RDP to logon to the box. Just launch RDP by using the Windows search function for RDP and then use the IP-address, and the admin credentials to initiate a RDP session. After having logged on succesfully you can see an almost fully black desktop with an open DOS console. We can just type in "explorer" in the console and confirm with Enter, then the regular desktop environment will be launched with the typical Windows task bar at the bottom.

![image](/assets/2025-04-10/040.png)

## Exchanging files with the box

If we need to exchange files with the box (e.g. for installing an ODBC driver), we just use the share "\\{Boxname}\Share" to move the file from a remote machine to the box. We can easily access it then from within our RDP or PowerShell session.

## Conclusion

We learned how to access the box via PowerShell and Remote Desktop connection. One again, we must understand that doing so might have a negative impact on safety and stability when done in a wrong way.

