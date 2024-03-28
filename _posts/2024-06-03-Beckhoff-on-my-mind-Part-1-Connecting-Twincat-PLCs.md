---
layout: post
title: Beckhoff on my mind - Part 1 - Connecting Twincat PLCs
date: 2023-03-01 12:00:00 +0200
tags: hardware twincat
image: /assets/2024-06-03/title.png
read_more_links:
  - name: Youtube Video Series about Programming Twincat 3
    url: https://www.youtube.com/watch?v=WkNOm-hMH3k&list=PLimaF0nZKYHz3I3kFP4myaAYjmYk1SowO&ab_channel=JakobSagatowski
  - name: Beckhoff Information System (aka the help site)
    url: https://infosys.beckhoff.com/
  - name: Part 2 - Writing back to Twincat PLCs
    url: /Beckhoff-on-my-mind-Part-2-Writing-back-to-Twincat-PLCs.html
downloads:
  - name: TwincatFirstSteps.pbmx
    url: /assets/2024-06-03/TwincatFirstSteps.pbmx
---
Twincat is one of the world's most popular PLCs, made by the German company Beckhoff. This article serves as a basic introduction for connecting Beckhoff Twincat 3 to Peakboard applications. We will also cover some basic knowledge about how Twincat works in the backend. 

## Installation and first steps

Different readers might have completely different levels of knowledge. In this article, we expect you to have an instance of Twincat up and running, with a basic program on it. The program must have some variables that are accessible from the outside.

If you are a complete newbie and do not meet these prerequisites yet, there's a nice video series available on YouTube called [PLC programming using TwinCAT 3](https://www.youtube.com/watch?v=WkNOm-hMH3k&list=PLimaF0nZKYHz3I3kFP4myaAYjmYk1SowO&ab_channel=JakobSagatowski). Besides the basic programming knowledge, the creator of this series explains how to install a Twincat instance, run it, and use a development environment to put some basic programs on it.

Beside the video series the regular [Beckhoff help site](https://infosys.beckhoff.com/) is also a good point to seek for any kind of help around issues that might pop up during the journey.

## Configuring the router for development

Let's assume we run the PLC instance on one remote machine and the development environment (XAE Shell for Twincat and Peakboard Designer) runs on the local computer. In that scenario we can't simply connect the "client" with the "server" we would expect from the experience with other sources. We need to configure a route first. To access the routes for both the dev machine and the PLC machine we can use the task tray icon (see screenshot). If the PLC machine is Linux based or a physical Beckhoff device, it comes with a web interface where you can do the same.

![image](/assets/2024-06-03/010.png)

Let's start with the local dev machine. When we add a route and click on broadcast search it is supposed to find our remote PLC in the network (the name of the sample remote PLC starts with "Desktop", but it's a remote machine anyway, sorry for the bad naming). If the broadcast cannot find it we help him by providing the ip address. It's important to change the "Remote Route" to "None / Server". Depending on the PLC configuration we need to provide user name and password. If we just set up a Twincat instance under Windows like it's explained in the video series, we leave all authentification blank. We also note the AMS ID we can see there because we will need it later.

![image](/assets/2024-06-03/020.png)

After having added the route we can see it in the list. If not, hitting "Refresh" will help. As we can see, the column "Connected" is empty.

![image](/assets/2024-06-03/021.png)

Now let's switch to the remote PLC side and add a route there that points to our dev machine.

![image](/assets/2024-06-03/030.png)

After having added the second route, the route entry is supposed to jump to "Connected" on both sides. That's the sign, that both machine know each other and can interact. The two screenshots show the router after having added the router on both machines.

![image](/assets/2024-06-03/031.png)

![image](/assets/2024-06-03/040.png)

## The program on the PLC

For our sample, we use a very simple program that runs on the remote PLC. It has two variables: A simple counter and a static string. The screenshot shows how the program looks like in debugging mode. The counter constantly changes when the program is running.

![image](/assets/2024-06-03/050.gif)

## Setting up the Peakboard data source

Let's switch to the Peakboard Designer and create a Beckhoff Twincat 3 data source. There are two main attributes to provide: The AMS ID (of the remote PLC as noted earlier) and the port. The default port is 851. If there's more than one program running on the PLC the second and third program comes on port 852, 853 etc....
After providing these attributes we can load the modules of the program we're accessing. The variables were interested in are available in he MAIN program. Hitting the refresh button then lists the variables as columns of the resultset. 

![image](/assets/2024-06-03/060.png)

Here's the final result when using a simple table to show the resultset

![image](/assets/2024-06-03/061.gif)

## Dynamic local router

Up to this point things seem pretty easy. However the problem is, that the target Peakboard runtime instance (e.g. a physical box or BYOD instance) most likely doesn't have a router like the local dev machine. So the application we built up to here won't run on a box. The Beckhoff data source foresees this problem and give the user the option to establish a temporary router on the fly to replace the router we configured earlier.
The screenshot shows how it works. To use the dynamic router we need to provide the local AMS ID (it must match with the AMS ID that is used on the PLC side to configure the counterpart of this router), along with the IP address of the remote PLC.
What Peakboard does now is to start the temporary router before the connection is established. 

![image](/assets/2024-06-03/070.png)

Atenttion!!!!
Here are the three points to make sure it works. If these points are not fullfilled, the tempory router won't work:

* If there's still a static router running on the local dev machine, we shut it down (by ending the local Twincat Windows services). Otherwise there are two routers running, which is not possible.
* To make the conection running on a box it's necessary to confiure the PLC router accordingly and provide the IP address of the box along with the AMS ID, and NOT ONLY the ip address of the dev machine.
* The ports 48899 (discovery), 48898 (unsecure) and 8016 (secure) should be open on both the PLC side and Peaboard runtime side. These ports are used to let the routers (both static and dynmic) communicate with each other.

## conclusion

Bekchoff Twincat 3 is a tricky issue in context of Pekaboard because the way Peakboard works does not perfectly fit to the architecture of routes in the Twincat context. Every Peakboard developer or Peakboard Designer user must precisely understand how it works, otherwise the connection won't work. Even if it works on the dev machine, it's not necessarily made sure it works on the box without having understood exactly what to configure.

