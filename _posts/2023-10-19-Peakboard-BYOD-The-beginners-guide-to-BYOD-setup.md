---
layout: post
title: Peakboard BYOD - The beginner's guide to BYOD setup
date: 2023-03-01 12:00:00 +0200
tags: administration bestpractice
image: /assets/2023-10-19/title.jpg
---

The physical black box has been the ulimate symbol for Peakboard for years. But as by the end of 2024 Peakboard offers their so called BYOD edition, Bring You Own Device. In this articles we will cover the most important things to know on how to setup your own device and turn it into a standalone runtime instance.

## Minimum requirements

For the operating system Peakboard runs on 

- Windows 10 (starting from version 1607), any edition, x64, (x86/ARM is not supported) 
- Windows 11, any edition, x64 (x86/ARM is not supported)

The sizing of the machine heavily depends on the use case you want to run, so it's a minimum requirement to have a CPU of 1 GHz (minimum 2 cores), 4gb RAM, 64gb storage. Please be generious and consider these numbers as the bare minimum just to get it up and running. For a smooth use experience at least double the numbers.

On the operating system there must be at least the [ASP.NET Core 3.1 Runtime (v3.1.32) - Windows Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-3.1.32-windows-hosting-bundle-installer) installed.

## Run the Setup

The setup itself is straightforward and offers an option to launch the runtime automatically. Usually this is the most common behaviour:

![image](/assets/2023-10-19/010.png)

Beside this the setup offers an option to apply the license key direcly during setup. If you don't have a license key at the moment of installation, just skip this step and apply the license later. The runtime instance will work well except for a license hint on the screen. No need to worry during setup.

![image](/assets/2023-10-19/020.png)

The setup will come with the regular desktop runtime application plus the management service and webserver service. We will cover some more insights on the runtime components in another article.

## Licensing and adding the instance to the designer

Make sure that the runtime instance is running properly. Then you can add the instance to the designer. For the default credentials please use "pbadmin" with password "p@ssw0rd".

If the instance is still unlicensed there's an information provided about that along with the option to submit a license.
To request a license from the Peakboard sales team, please send the hardware key by mail. They will send you a license key and a box ID in return. The license key is a cryptic key (around 20-30 characters long) and the box ID usually looks like PBRXXXXX, while XXXX is a random number.
Feel free to skip this step and leave the instance unlicensed if you only want to test it. And unlicensed box instance work fine without any restriction except for a banner showing information about the missing license.

![image](/assets/2023-10-19/030.png)

## Automatic login

In common use cases the runtime is supposed to launch automatically after the device is switched on. So the first step is perform a windows logon automatically after boot. This is done through a registry key. Ideally there's a local Windows account on the machine used for the automatic login. You can run this PowerShell script to set the registry key for the automatic logon by providing user name and password.

{% highlight powershell %}
$Username = Read-Host 'Enter username for auto-logon (f.e. domain\peakboard)'
$Pass = Read-Host "Enter password for $Username"
$RegistryPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon'
Set-ItemProperty $RegistryPath 'AutoAdminLogon' -Value "1" -Type String 
Set-ItemProperty $RegistryPath 'DefaultUsername' -Value "$Username" -type String 
Set-ItemProperty $RegistryPath 'DefaultPassword' -Value "$Pass" -type String
{% endhighlight %}

To launch the runtime automatically after automatic login just place a shortcut for the Peakboard.Runtime.Wpf.exe in the user's directory of %appdata%\Microsoft\Windows\Start Menu\Programs\Startup (e.g. C:\Users\michelle\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup).
The setup has done this for you, if you have chosen the option during the installation process.

## Making things even smoother

The steps described above are the main steps to get it working on a regular device. However maybe you want to make the user experience more smooth and hide "windows" away from the end users. Here are some steps to do this:

* Turn the desktop background black without any images
* Clean up the desktop and remove all shortcuts
* Hide the command bar
* Upload a black profile picture for the account on which the runtime instance runs






