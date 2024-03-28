---
layout: post
title: Beckhoff on my mind - Part 1 - Connecting Twincat PLCs
date: 2023-03-01 12:00:00 +0200
tags: hardware twincat
image: /assets/2024-06-03/title.png
read_more_links:
  - name: Youtube Video Series about Programming TwinCAT 3
    url: https://www.youtube.com/watch?v=WkNOm-hMH3k&list=PLimaF0nZKYHz3I3kFP4myaAYjmYk1SowO&ab_channel=JakobSagatowski
  - name: Beckhoff Information System (the help site)
    url: https://infosys.beckhoff.com/
  - name: Part 2 - Writing back to TwinCAT PLCs
    url: /Beckhoff-on-my-mind-Part-2-Writing-back-to-Twincat-PLCs.html
downloads:
  - name: TwincatFirstSteps.pbmx
    url: /assets/2024-06-03/TwincatFirstSteps.pbmx
---
Twincat is one of the world's most popular PLCs, made by the German company Beckhoff. This article serves as a basic introduction for connecting Beckhoff Twincat 3 to Peakboard applications. We will also cover some basic knowledge about how Twincat works in the backend. 

## Installation and first steps

In this article, we expect you to have an instance of Twincat up and running, with a basic program on it. The program must have some variables that are accessible from the outside.

If you are a complete newbie and do not meet these prerequisites yet, there's a nice video series available on YouTube called [PLC programming using TwinCAT 3](https://www.youtube.com/watch?v=WkNOm-hMH3k&list=PLimaF0nZKYHz3I3kFP4myaAYjmYk1SowO&ab_channel=JakobSagatowski). Besides some basic programming knowledge, this series also explains how to install a Twincat instance, run it, and use a development environment to put some basic programs on it.

Aside from this video series, the [Beckhoff help site](https://infosys.beckhoff.com/) is also a good place to look for help with any issues that might pop up on your journey.

## Configure the router for development

Let's assume we run the PLC instance on a remote machine, and the development environment (XAE Shell for Twincat and Peakboard Designer) runs on the local computer. In this scenario, we can't simply connect the "client" with the "server," like we would with other sources. Instead, we need to configure a route first.

To access the routes for both the dev machine and the PLC machine, we can use the task tray icon (see the following screenshot). If the PLC machine is a Linux-based machine or a physical Beckhoff device, it comes with a web interface where you can do the same thing.

![image](/assets/2024-06-03/010.png)

Let's start with the local dev machine. When we add a route and click on **Broadcast Search**, it's supposed to find our remote PLC in the network. (The name of the remote PLC starts with "Desktop," but it's actually a remote machine. Sorry for the bad naming.)

If the broadcast search cannot find it, then we help it by providing the IP address. It's important to change **Remote Route** to **None / Server**. Depending on the PLC configuration, we may also need to provide the username and password. If we set up a Twincat instance under Windows, like it's explained in the video series, then we leave all authentication blank.

We also copy down the AMS ID that's displayed, because we'll need it later.

![image](/assets/2024-06-03/020.png)

After having added the route, we can see it in the list. If it's not there, hitting **Refresh Status** will help. As you can see, the **Connected** column is empty.

![image](/assets/2024-06-03/021.png)

Now let's switch to the remote PLC side and add a route there that points to our dev machine.

![image](/assets/2024-06-03/030.png)

After having added the second route, the route entry should to jump to **Connected** on both sides. That's the sign that both machines know each other and can interact. The following two screenshots show the router after having added the router on both machines:

![image](/assets/2024-06-03/031.png)

![image](/assets/2024-06-03/040.png)

## The program on the PLC

For our example, we use a very simple program that runs on the remote PLC. It has two variables: A simple counter and a static string. The screenshot shows what the program looks like in debugging mode. The counter constantly changes when the program is running.

![image](/assets/2024-06-03/050.gif)

## Set up the Peakboard data source

Let's switch to Peakboard Designer and create a Beckhoff Twincat 3 data source. There are two main attributes to provide:
* The AMS ID of the remote PLC, as noted earlier.
* The port. The default port is 851. If there's more than one program running on the PLC, the second and third program comes on port 852, 853, etc.

After providing these attributes, we can load the modules of the program we're accessing. The variables we're interested in are available in the MAIN program. Hitting the refresh button lists the variables as columns of the result set. 

![image](/assets/2024-06-03/060.png)

Here are the final results when using a simple table to show the result set:

![image](/assets/2024-06-03/061.gif)

## Dynamic local router

Up to this point, things have been pretty easy. However, the target Peakboard runtime instance (a physical box or BYOD instance) probably doesn't have a router like the local dev machine. So the application we've built up to this point won't run on a Box.

The Beckhoff data source handles this problem by providing the option to establish a temporary router on the fly, to replace the router we configured earlier.

The following screenshot shows how it works. To use the dynamic router, we need to provide two things:
* The local AMS ID (it must match with the AMS ID that is used on the PLC side, to configure the counterpart of this router).
* The IP address of the remote PLC.

Then, Peakboard starts the temporary router before the connection is established. 

![image](/assets/2024-06-03/070.png)

### Attention!
Here are three things we need to do, to make sure the temporary router works. If these things are not done, the router won't work:
* If there's still a static router running on the local dev machine, we shut it down (by ending the local Twincat Windows services). Otherwise, there are two routers running, which is not possible.
* To have the connection run on a Box, it's necessary to configure the PLC router accordingly and provide the IP address of the box, along with the AMS ID---*not just* the IP address of the dev machine.
* The ports 48899 (discovery), 48898 (unsecure) and 8016 (secure) should be open on both the PLC side and Peakboard runtime side. These ports are used to let the routers (both static and dynamic) communicate with each other.

## Conclusion

Bekchoff Twincat 3 is a tricky issue in the context of Pekaboard, because the way Peakboard works does not perfectly fit the architecture of routes in the Twincat context. Every Peakboard developer and designer must precisely understand how it works. Otherwise, the connection won't work. Even if it works on the dev machine, it won't necessarily work on the Box, if it's not configured properly.