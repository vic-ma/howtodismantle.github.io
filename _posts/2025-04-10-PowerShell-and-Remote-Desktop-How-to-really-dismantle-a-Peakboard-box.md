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

Here are some examples of things you can do with administrative privileges:

- Install Windows updates
- Install additional software to align the computer with internal security compliance guidelines
- Install additional software for an application that runs on the Peakboard runtime (e.g. ODBC drivers)
- Track and monitor error events in the event log
- Manually install Peakboard runtime updates
- Bring the device into the AD domain

Before we step into the technical details, you must understand that accessing the Peakboard Box with administrative-level credentials might pose a security risk, and it might reduce the stability of the system. So you should only engage in these activities when necessary.

We will use Remote Desktop to manage a Peakboard Box. But before we can do that, we first need to prepare the Peakbord Box for Remote Desktop, by using PowerShell.

## Prepare for Remote Desktop with PowerShell

First, run PowerShell as an administrator:

![image](/assets/2025-04-10/010.png)

Then, run these commands:

{% highlight powershell %}
net start winrm
Set-Item WSMan:\localhost\Client\TrustedHosts -Value {IP}
Enter-PSSession -ComputerName {IP} -Credential {Box Hostname}\pbadmin
{% endhighlight %}

Here's an explanation:

1. We start the `winrm` service, because we need it to manage remote administration task on other machines (like our Peakboard Box).
2. We whitelist the IP address of the Peakboard Box.
3. We start a remote session in the Peakboard Box. There is a prompt for the adminstrator's password.

![image](/assets/2025-04-10/020.png)

![image](/assets/2025-04-10/025.png)

After having successfully established the PS session, we change a value in the registry of the Box to enable remote desktop connections. (It's disabled by default.) We also open the port for remote desktop in the firewall. Then, we exit the remote session and close the PS console.

{% highlight powershell %}
Set-ItemProperty -Path 'HKLM:\System\CurrentControlSet\Control\Terminal Server' -name "fDenyTSConnections" -value 0
Enable-NetFirewallRule -DisplayGroup "Remote Desktop"
exit
exit
{% endhighlight %}

![image](/assets/2025-04-10/030.png)

## Log in with Remote Desktop

Now, we can log into the Peakboard Box with Remote Desktop. Here are the steps:

1. Launch Remote Desktop by using the Windows search function. 
2. Use the IP address and the admin credentials of the box to initiate a Remote Desktop session. After having logged on successfully, you will see a black desktop with a single terminal window open.
3. Type `explorer` into the terminal and confirm with Enter, then the regular desktop environment will be launched with the typical Windows task bar at the bottom.

![image](/assets/2025-04-10/040.png)

## Exchanging files with the box

If we need to exchange files with the box (e.g. for installing an ODBC driver), we just use the share "\\{Boxname}\Share" to move the file from a remote machine to the box. We can easily access it then from within our RDP or PowerShell session.

## Conclusion

We learned how to access the box via PowerShell and Remote Desktop connection. One again, we must understand that doing so might have a negative impact on safety and stability when done in a wrong way.

