---
layout: post
title: Peakboard Meets BACnet – Your First Steps in Building Automation
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
Peakboard version 4.1 introduced a new built-in data source for BACnet. BACnet (Building Automation and Control Network) is an open communication protocol designed for building automation systems, and it is widely used in large buildings, factories, and smart infrastructures for centralized monitoring and control of energy systems. In this article we will delve deeper into the question of how to connect a Peakboard application to BACnet.

## Set up test environment

Let's assume we don't have access to a BACnet-capable device, or the devices we do have access to are not meant to be used for experimental purposes. There's an easy way to set up a simple, local test environment and take the chance to learn all aspects of BACnet without the need to have full access to physical devices.

The tool [Yet Another BACnet Explorer – or just YABE –](https://sourceforge.net/projects/yetanotherbacnetexplorer/) is an open source utility for connecting to BACnet devices and offers an explorer-style UI to connect to and explore devices in the network. It comes with three different simulators that expose BACnet endpoints for testing. From the YABE main window the simulators can be launched through `Options` -> `User commands`. In our example we will use the room controller. It simulates a heating/cooling system. The screenshot shows the YABE explorer on the left and the simulator on the right. It finds the device automatically through network broadcast and shows all attributes of an object in the list on the lower left.

Attributes (e.g. the temperature) can be subscribed and tracked in the middle part of the window.

![YABE room controller simulator and explorer interface](/assets/2025-12-13/bacnet-yabe-room-controller-simulator.png)

## Set up the data source in Peakboard

Let's switch to the Peakboard side. This data source is not pulling the data on a regular basis but is built on a push architecture. This means that data is actively pushed by the device to the application. Therefore we set `Reload State` to `Subscription`. The callback port is by default set to `47808`, and usually there's no need to change it.

The data source supports connecting to a single device by using a dedicated IP address, port, and BACnet device ID. However, in our case we take the more common approach of using the subscription option for multiple devices. When we click on `Manage Subscriptions` the dialog is designed to find all available devices in the network automatically and lets us pick the data points we want to subscribe to. In our case we just take all of those. On the right side we can dig deeper into BACnet specific information about each data point.

![Peakboard BACnet subscription dialog showing available devices](/assets/2025-12-13/peakboard-manage-bacnet-subscriptions.png)

After setting the subscription we can click on data refresh and let the device fill the result set with test data.

![Peakboard BACnet data preview filled with test values](/assets/2025-12-13/peakboard-bacnet-data-preview.png)

## Process the data

The data is a list where the name of the data point is actually the key within the list. However we can't rely on the row index of a certain data point. So let's say the temperature data might be at row index 0 one day and on row index 2 on another day. To reliably get the data point for later usage, we just build a data flow to filter the list. The screenshot shows the data flow for the indoor temperature. Besides the filter we also adjust the data type to `Number`.

![Peakboard data flow filtering indoor temperature values](/assets/2025-12-13/peakboard-dataflow-filter-temperature.png)

With the data flow in place we can easily bind any control in our application to the data. Not only text fields but also icons or other controls.

![Peakboard control binding for BACnet indoor temperature](/assets/2025-12-13/peakboard-control-binding-example.png)

## Writing to the device

Let's have a look at how to send back commands to a BACnet device. The screenshot shows how to use a Building Block to set the value of an attribute on a device. We need to know the data type and also the instance ID of the property to be set. We can easily find those two pieces of information in the output list of the data source (just check the sample data).

![Peakboard Building Block writing a BACnet attribute value](/assets/2025-12-13/peakboard-building-block-write-attribute.png)

## Result

The video shows the preview in action along with the simulator. In our sample application we just list all properties in a table to show the raw data. We can see how the values are changing simultaneously. When we click on the `cold` or `hot` button the set temperature is adjusted accordingly in the simulator.

![Peakboard BACnet sample application preview video](/assets/2025-12-13/result.gif)



