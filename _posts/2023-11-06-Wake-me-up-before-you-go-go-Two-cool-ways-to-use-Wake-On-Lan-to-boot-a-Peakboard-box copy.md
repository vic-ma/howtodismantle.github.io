---
layout: post
title: Wake me up before you go - Two cool ways to use Wake-On-Lan to boot a Peakboard Box
date: 2023-03-01 12:00:00 +0200
tags: administration
image: /assets/2023-11-06/title.png
read_more_links:
  - name: Stack overflow for WoL with PowerShell
    url: https://stackoverflow.com/questions/72853502/how-to-send-a-wake-on-lan-magic-packet-using-powershell
  - name: Bed time stories - Three ways to shut down a Peakboard box remotely
    url: /Bed-time-stories-Three-ways-to-shut-down-a-Peakboard-box-remotely.html
---

In last week's [article](/Bed-time-stories-Three-ways-to-shut-down-a-Peakboard-box-remotely.html), we learned how to shut down a Peakboard Box. In this article, we will learn how to use [Wake-on-Lan](https://en.wikipedia.org/wiki/Wake-on-LAN) (WoL) to boot a Peakboard Box remotely. There are two different methods for doing so.

## MagicPacket

The first method is to use the tool [MagicPacket](https://apps.microsoft.com/detail/magicpacket/9WZDNCRCW1MX?hl=de-de&gl=DE). Microsoft provides MagicPacket for free, as a way to send WoL packets to a device. It integrates smoothly with Peakboard.

After installing the tool, just add a new computer. You only need to provide a name and the MAC address of the Peakboard Box's LAN adapter. The easiest way to get this is to use the "Connections" tab within the Box dialog in Peakboard Designer:

![image](/assets/2023-11-06/005.png)

Here's how to fill the MAC address field in MagicPacket:

![image](/assets/2023-11-06/010.png)

That's it! Now, just push the button to initiate the boot sequence and start up the Box:

![image](/assets/2023-11-06/020.png)

## Powershell

We can do the same thing from PowerShell. There are many ways to send the WoL signal. This script is the easiest way to do it without any additional scriptlets or functions. It also works with all kinds of MAC address formats:

{% highlight powershell %}
$mac = '00:E0:4C:0C:73:9C'; 
[System.Net.NetworkInformation.NetworkInterface]::GetAllNetworkInterfaces() | Where-Object { $_.NetworkInterfaceType -ne [System.Net.NetworkInformation.NetworkInterfaceType]::Loopback -and $_.OperationalStatus -eq [System.Net.NetworkInformation.OperationalStatus]::Up } | ForEach-Object { $targetPhysicalAddressBytes = [System.Net.NetworkInformation.PhysicalAddress]::Parse(($mac.ToUpper() -replace '[^0-9A-F]','')).GetAddressBytes(); $packet = [byte[]](,0xFF * 102); 6..101 | Foreach-Object { $packet[$_] = $targetPhysicalAddressBytes[($_ % 6)] }; $client = [System.Net.Sockets.UdpClient]::new([System.Net.IPEndPoint]::new(($_.GetIPProperties().UnicastAddresses | Where-Object { $_.Address.AddressFamily -eq [System.Net.Sockets.AddressFamily]::InterNetwork })[0].Address, 0)); try { $client.Send($packet, $packet.Length,[System.Net.IPEndPoint]::new([System.Net.IPAddress]::Broadcast, 9)) | Out-Null } finally { $client.Dispose() } }
{% endhighlight %}





