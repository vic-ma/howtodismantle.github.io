---
layout: post
title: Peakboard Meets BACnet – Your First Steps into Building Automation
date: 2023-03-01 00:00:00 +0000
tags: hardware
image: /assets/2025-12-13/title.png
image_header: /assets/2025-12-13/title.png
bg_alternative: true
read_more_links:
  - name: Yet Another BACnet Explorer
    url: https://sourceforge.net/projects/yetanotherbacnetexplorer/
downloads:
  - name: BACnetTestApp.pbmx
    url: /assets/2025-12-13/BACnetTestApp.pbmx
---
With Version 4.1 Peakboard introduced a new Built-Data source for BACnet. BACnet (Building Automation and Control Network) is an open communication protocol designed for building automation systems and it is widely used in large buildings, factories, and smart infrastructures for centralized monitoring and control of energy systems.
In this article we will step deeper into the question of how to connect a Peakboard application to BACnet.

## Set up test environment

Let's assume we don't have access to a BACnet capable device or the devices we do have access to are not meant to be used for experimental purpose. There's any easy way to set up a simple, local test environment and take the chance to learn all aspects of BACnet without the need to have full acces to physical devices.

The tool [Yet Another BACnet Explorer - or just YABE -](https://sourceforge.net/projects/yetanotherbacnetexplorer/) is an open source tool for connecting to BACnet devices and offers some kind explorer style UI to connect and explore devices in the network. It come with three different simulators that expose BARnet endpoints for testing. From the YABE main window the simulators can be launched through `Options` -> `User commands`. In our exmaple we will use the room controller. It simulates a heating/cooling system. The screenshot shows the YABE explorer on the left and the simulator at the right. It finds the device automatically through network broadcast and shows all attributes of an object in the list on the left bottom.

Attributes (e.g. the temperature) can be subscribed and tracked in the middle pat of the window.

![Image](/assets/2025-12-13/010.png)

## Set up the data source in Peakboard

Let's switch to the Peakboard side. Ths data source is not pulling the data on a regular basis but is built on a push architecture. This means that data is actively pushed by the device to the application. Therefore we set `Reload State` to `Subscription`. The callback port is by default set to `47808` and usually there's no need to change it.

The data source supports to connect to a single device by using a dedicated ip address, port and BACnet device ID. However in our case we just go the more common way of using the subscription option for multiple devices. When we click on `Manage Subscriptions` the dialog is supposed to find all available devices in the network automatically and let's us pick the data points we want to subscribe to. In our case we just take all of those. On the right side we can dig deeper into BACnet specific informations about each data point.  

![Image](/assets/2025-12-13/020.png)

After having set the subscription we can click on data refresh and let the device fill the result set with test data.

![Image](/assets/2025-12-13/030.png)

## Process the data

The data is a list where the name of the data point is actually the key within the list. However we can't rely on the row index of a certain data point. So let's say the temperature data might by at row index 0 one day and on row index 2 on another day. To reliabely get the data point for later usage, we just build a data flow to filter the list. The screenshot shows the data flow for the indoor temperature. Beside the filter we also adjust the data type to `Number`.

![Image](/assets/2025-12-13/040.png)

With the data flow in place we can easily bind any control in our application to the data. Not only text feilds but also incons or other controls.

![Image](/assets/2025-12-13/050.png)

## Writing to the device

Let's have a look on how to send back commands to a BACnet device. The screenshot shows how to use a Building Block to set the value of an attribute on a device. We need to know the data type and also the instance ID of the property to be set. We can easily find those two informations in the output list of the data source (just check the sample data).

![Image](/assets/2025-12-13/060.png)

## result

The video shows the preview in action along with the simulator. In the our sample application we just list all properties in table to show the raw data.  We can see how the values are changing simultaniusouly. WHen we click on the `cold` or `hot` button the set temperature is adjusted accordingly in the simulator.

![Image](/assets/2025-12-13/result.gif)



