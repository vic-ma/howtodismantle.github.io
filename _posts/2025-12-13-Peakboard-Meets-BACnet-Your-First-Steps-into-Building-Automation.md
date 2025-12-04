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
Peakboard version 4.1 introduced a new built-in data source for BACnet. BACnet (Building Automation and Control Network) is an open communication protocol designed for building automation systems. It's widely used in large buildings, factories, and smart infrastructure, for centralized monitoring and control of energy systems. In this article, we'll take a look at how to connect a Peakboard application to a device, with BACnet.

## Set up test environment

First, let's assume that we don't have access to a BACnet-capable device, or that the devices we do have access to are not meant for testing purposes. So, we need to set up a BACnet test environment that lets us experiment with BACnet.

To do this, we use [Yet Another BACnet Explorer (YABE)](https://sourceforge.net/projects/yetanotherbacnetexplorer/). YABE is an open-source tool for connecting to and exploring BACnet devices. However, it also comes with three different simulators that expose BACnet endpoints for testing. To launch a simulator, we go to *Options > User commands*.

For our example, we'll use the room controller simulator. It simulates a heating/cooling system. We go to *Options > User commands* and launch it. Soon after, YABE automatically finds the simulated device and lists all of its attributes in the bottom-left pane. You can subscribe to and track specific attributes (like the temperature) from the top-middle pane.

Here, you can see the YABE explorer on the left and the simulator on the right:
![YABE room controller simulator and explorer interface](/assets/2025-12-13/bacnet-yabe-room-controller-simulator.png)

## Create the Peakboard app

Now that our test environment is set up, let create the Peakboard app!

### Add the BACnet data source

First, we add a new BACnet data source. This data source uses a publish-subscribe pattern. This means that the data source subscribes to the BACnet device and waits for the device to send data. Because of this, we set *Reload State* to *Subscription*.

The callback port is set to 47808 by default, and there's usually no need to change it.

To connect to our BACnet device, we have two options:
1. We set *Subscriptions* to *Single Device* and we manually enter the IP address, port, and BACnet device ID of our BACnet device.
1. We set *Subscriptions* to *Multi Devices* and we use the subscriptions manager to scan the network automatically. Then, we select the data points that we want to subscribe to.

The *Multi Devices* option is usually the right choice (even if we only have one device), because it's easier to use.

So, we set *Subscriptions* to *Multi Devices* and we click *Manage subscriptions*. Then, we select all the data points of our simulated device.

![Peakboard BACnet subscription dialog showing available devices](/assets/2025-12-13/peakboard-manage-bacnet-subscriptions.png)

After setting the subscription, we can click on the refresh button and verify that the preview window now contains data from the simulated device:

![Peakboard BACnet data preview filled with test values](/assets/2025-12-13/peakboard-bacnet-data-preview.png)

### Process the data

The data is a list where the name of the data point is actually the key within the list. However we can't rely on the row index of a certain data point. So let's say the temperature data might be at row index 0 one day and on row index 2 on another day. To reliably get the data point for later usage, we just build a data flow to filter the list. The screenshot shows the data flow for the indoor temperature. Besides the filter we also adjust the data type to `Number`.

![Peakboard data flow filtering indoor temperature values](/assets/2025-12-13/peakboard-dataflow-filter-temperature.png)

With the data flow in place we can easily bind any control in our application to the data. Not only text fields but also icons or other controls.

![Peakboard control binding for BACnet indoor temperature](/assets/2025-12-13/peakboard-control-binding-example.png)

### Write to the BACnet device

Let's have a look at how to send back commands to a BACnet device. The screenshot shows how to use a Building Block to set the value of an attribute on a device. We need to know the data type and also the instance ID of the property to be set. We can easily find those two pieces of information in the output list of the data source (just check the sample data).

![Peakboard Building Block writing a BACnet attribute value](/assets/2025-12-13/peakboard-building-block-write-attribute.png)

## Result

The video shows the preview in action along with the simulator. In our sample application we just list all properties in a table to show the raw data. We can see how the values are changing simultaneously. When we click on the `cold` or `hot` button the set temperature is adjusted accordingly in the simulator.

![Peakboard BACnet sample application preview video](/assets/2025-12-13/result.gif)



