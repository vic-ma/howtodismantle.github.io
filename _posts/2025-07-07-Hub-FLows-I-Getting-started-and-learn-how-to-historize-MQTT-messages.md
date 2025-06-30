---
layout: post
title: Hub Flows I - Getting started and learning how to historicize MQTT messages
date: 2023-03-01 00:00:00 +0000
tags: hubflows
image: /assets/2025-07-07/title.png
image_header: /assets/2025-07-07/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles around Hub Flows
    url: /category/opcuamqtt
downloads:
  - name: TemperatureFlow.pbfx
    url: /assets/2025-07-07/TemperatureFlow.pbfx
---
Peakboard has always focused on UI-oriented, local applications---unlike other systems like Tulip or Mendix. With Peakboard, all the processing happens on a local device (a Box or a BYOD instance), so you don't need to have central servers or a cloud to host your applications. This is a big advantage over the alternative solutions, and it's one of the main reasons why so many companies and developers choose Peakboard.

But there *are* some use instances where it makes sense to have a central server host an application. That's why the Peakboard development team recently launched *Peakboard Hub Flows*. 

This article is the first in a series of articles that explain how to use Hub Flows. This article serves as a starting point and explains all the use cases that we will cover in this series. We'll start with the simple task historicizing MQTT messages in a Hub list. Here's a list of all use cases covered in this series:

1. [Getting started and learn how to historicize MQTT messages](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html)
2. [Cache Me If You Can - Data Distribution and caching for SAP Capacity Data](/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html)
3. [One PLC to Feed Them All: Using Peakboard Flows to Share and Distribute Siemens S7 Values](/Hub-Flows-III-One-PLC-to-Feed-Them-All-Using-Peakboard-Flows-to-Share-and-distribute-Siemens-S7-Values.html)
4. [Peakboard Flows in Production: Asynchronous SAP Confirmation Processing](/Hub-Flows-IV-Peakboard-Flows-in-Production-Asynchronous-SAP-Confirmation-Processing.html)
5. [Condense, Archive, Optimize: Use Hub Flows to Pre-Aggregate and Archive High-Volume Transaction Data](/Hub-Flows-V-Condense,-Archive-Optimize-Use-Hub-Flows-to-Pre-Aggregate-and-Archive-High-Volume-Transaction-Data.html)

## Understand the idea of Hub Flows

To build a Hub Flow, you use the Peakboard Designer. And to host and run a Hub Flow, you use Peakboard Hub. Before you build a Hub Flow, you must first connect Peakboard Designer to a Peakboard Hub instance.

Starting with Peakboard Designer version 4, there's a new project type called a *Flow project*:

![image](/assets/2025-07-07/005.png)

A Flow project doesn't have screens, because Hub Flows don't have UIs. But, Flow projects do share many similarities with regular Designer projects:
* Flow projects use data sources and dataflows to retrieve and process data from connected systems.
* Flow projects use functions (either with LUA code or with Building Blocks) for complex application logic
* Flow projects use Hub Flows to coordinate actions and to control the application. So in a Flow project, you always need a Hub Flow needed to execute a data source, a dataflow, or a function.
  Unlike standard Designer projects, data sources and dataflows are not executed automatically. And there are no automatic reload scripts. So you always need a Hub Flow to trigger them. This makes Hub Flows the sole tool for defining and coordinating what a Flow project does and under what conditions.

The final thing you need to know about is the role of Hub lists. You can use Hub lists just like a regular table, to store and read data. However, it's very easy to store data in Hub lists, within a flow project. There's built-in functionality to store data into Hub lists that come from data sources or dataflows. We'll take a look at, later in this article. 

## First steps with Hub Flows

For our first example project, we will build and deploy a Hub Flow that provides temperature data. We open Peakboard Designer and select *New Flow project* to get started.

### Temperature data source

First, we create a MQTT data source. It receives a temperature value. And we store information in a Hub table, for long-term historicization. We've discussed how the [temperature sensor](/Peakboard-Meets-Shelly-Building-a-Smart-Dashboard-for-Tracking-Temperature-and-Humidity.html) works in another article, so we won't explain how to set it up here.

This screenshot shows that our MQTT data source is subscribed to a single MQTT node, which is the temperature:

![image](/assets/2025-07-07/010.png)

### Timestamp dataflow

We want to add a timestamp to our temperature value. So, we add a dataflow to our data source. It works like this:
1. Add a column with the current time stamp.
2. Change the order of the columns to have the timestamp appear before the temperature. This looks a little bit nicer.

![image](/assets/2025-07-07/020.png)

### Build the Hub Flow

Finally, we build our Hub Flow:
1. Create a new Hub Flow.
2. Add a trigger. We want the Hub Flow to be triggered every time a new MQTT message is sent by our thermostat and received by our data source. So the trigger is the list refresh of our data source.
3. Add the steps for the Hub Flow:
    1. Load the timestamp dataflow that we created. Remember that in Flow projects, dataflows are not triggered automatically. That's why we have to manually run the dataflow here, inside the Hub Flow.
    2. Store the output of the dataflow in a Hub list. There's no more work to do in this step, because the Hub Flow automatically creates and names the Hub list, by using metadata.

![image](/assets/2025-07-07/030.png)

## Deploy and monitor the first Hub Flow

It's a requirement that the designer is already connected to a valid Hub. Then we can just hit the upload button to deploy and activate the Flow.

![image](/assets/2025-07-07/040.png)

When we switch over to the Hub portal we can find the Flow tab for monitoring any Flow that is deployed to the Hub. For every flow we can see the last execution visually and also browse through the log entries. In our sample the last incoming MQTT message was successfully processed.

![image](/assets/2025-07-07/050.png)

When we switch to the "Lists" tab, we can find the autmatically generated Hub list where the data is stored. We see, that it was not necessary to create the list manually. It was generated autmatically. The screenshot shows, that the first two temperture values were already stored correctly.

![image](/assets/2025-07-07/060.png)

## result and conclusion

We finished out first walkthough and can understand all basic concepts of the hub. We can use tradional elements like data sources and data flows and then use Hub Flows to put everything together and design the actual process. Storing data in a hub list is one of the key elements and is done through a built in function.

