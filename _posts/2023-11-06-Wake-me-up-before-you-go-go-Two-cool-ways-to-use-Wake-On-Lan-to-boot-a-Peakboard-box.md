---
layout: post
title: Wake me up before you go go - Two cool ways to use Wake-On-Lan to boot a Peakboard box
date: 2023-03-01 12:00:00 +0200
tags: administration
image: /assets/2023-11-06/title.jpg
read_more_links:
  - name: Stack overflow for WoL with PowerShell
    url: https://stackoverflow.com/questions/72853502/how-to-send-a-wake-on-lan-magic-packet-using-powershell
  - name: Bed time stories - Three ways to shut down a Peakboard box remotely
    url: /Bed-time-stories-Three-ways-to-shut-down-a-Peakboard-box-remotely.html
---

In last week's [article](/Bed-time-stories-Three-ways-to-shut-down-a-Peakboard-box-remotely.html) we learned how to shut down Peakboard boxes. In this article we will learn how to use [Wake-on-Lan](https://en.wikipedia.org/wiki/Wake-on-LAN) to boot them remotely by using two different methods.

## MagicPacket

The tool [MagicPacket](https://apps.microsoft.com/detail/magicpacket/9WZDNCRCW1MX?hl=de-de&gl=DE) is provided by Microsoft for free to send WoL packets to a device. It works smoothly together with Peakboard. After installing the tool just add a new computer. You only need to provide a name and the MAC-address of the Peakboard box's LAN adapter. The easiest way to get it is to use the "Connections" tab within the box dialog in Peakboard designer:

![image](/assets/2023-11-06/005.png)

Here's how to fill the MAC address field in MagicPacket

![image](/assets/2023-11-06/010.png)

That's it! Now just push the button to initiate the boot sequence and start upthe box:

![image](/assets/2023-11-06/010.png)

## Powershell

The same works well from PowerShell. There are many way to send the the WoL signal. This script is the easiest to do with any addtional scriptlets or function. It also work with all kinds of MAC address formats:

{% highlight powershell %}
$mac = '00:E0:4C:0C:73:9C'; 
[System.Net.NetworkInformation.NetworkInterface]::GetAllNetworkInterfaces() | Where-Object { $_.NetworkInterfaceType -ne [System.Net.NetworkInformation.NetworkInterfaceType]::Loopback -and $_.OperationalStatus -eq [System.Net.NetworkInformation.OperationalStatus]::Up } | ForEach-Object { $targetPhysicalAddressBytes = [System.Net.NetworkInformation.PhysicalAddress]::Parse(($mac.ToUpper() -replace '[^0-9A-F]','')).GetAddressBytes(); $packet = [byte[]](,0xFF * 102); 6..101 | Foreach-Object { $packet[$_] = $targetPhysicalAddressBytes[($_ % 6)] }; $client = [System.Net.Sockets.UdpClient]::new([System.Net.IPEndPoint]::new(($_.GetIPProperties().UnicastAddresses | Where-Object { $_.Address.AddressFamily -eq [System.Net.Sockets.AddressFamily]::InterNetwork })[0].Address, 0)); try { $client.Send($packet, $packet.Length,[System.Net.IPEndPoint]::new([System.Net.IPAddress]::Broadcast, 9)) | Out-Null } finally { $client.Dispose() } }
{% endhighlight %}





