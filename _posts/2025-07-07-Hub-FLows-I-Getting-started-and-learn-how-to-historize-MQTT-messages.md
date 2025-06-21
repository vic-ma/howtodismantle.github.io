---
layout: post
title: Hub Flows I - Getting started and learn how to historize MQTT messages
date: 2023-03-01 00:00:00 +0000
tags: hubflows
image: /assets/2025-07-07/title.png
image_header: /assets/2025-07-07/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles around Hub FLows
    url: /category/opcuamqtt
downloads:
  - name: TemperatureFlow.pbfx
    url: /assets/2025-07-07/TemperatureFlow.pbfx
---
Different from other systems like Tulip or Mendix, Peakboard always focused on UI oriented, local applications. All the magic and logic happens on a local device - box or BYOD instance, without the fundamental need for central servers or a cloud to host the application logic. That's a huge advantage before the other systems and one of the main reasons why Peakboard is choosen by so many developers and users. But there are some use cases where it makes sense to rely on a central system and host some logic independant from an indivual workplace. That's why the Peakboard dev team introduced the Peakboard Hub Flows in summer 2025. 
This article is a starting point to a series of articles to introduce Hub Flows and explain the use cases which can be covered. We will start a very simple task to historize MQTT messages in a Hub list. Here's a list of all use cases covered in this series:

1. [Getting started and learn how to historize MQTT messages](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html)
2. [Cache Me If You Can - Data Distribution and caching for SAP Capacity Data](/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html)
3. [One PLC to Feed Them All: Using Peakboard Flows to Share and Distribute Siemens S7 Values](/Flows-III-One-PLC-to-Feed-Them-All-Using-Peakboard-Flows-to-Share-and-distribute-Siemens-S7-Values.html)
4. Peakboard Flows in Production: Asynchronous SAP Confirmation Processing
5. Condense, Archive, Optimize: Use Hub Flows to Pre-Aggregate and Archive High-Volume Transaction Data

## Understanding the idea of Hub FLows

Hub Flows are designed and built in the Peakboard designer, and then run and hosted in the Peakboard Hub. So the upload of the project to the Hub can be compared with uploading a regular application to a Peakboard box. It's a requirement to connect the designer to a Hub instance first. Starting from Designer version 4 there's a new project type called "Flow project". A FLow project doesn't have screens, because it doesn't have any UI when running. But there are many other artifacts we aleady know from regular designer projects.

We use Data Sources and Data FLows to retrieve and process data from connected systems. We can use functions (either with LUA code or with Building Blocks) for complex application logic, and - most importantly - we use Flows to coordinate all actions and to control the actual application process. So within the Flow project, there's always a FLow needed to execute a Data Source, a Data FLow or a function. Different from regular UI projects Data Sources and Data FLows are not excuted automatically. And there are no automatic Reload Scripts. We always need a flow to trigger them. That makes the Flows the ulimate linchpin to define and coordinate what the Flow project is actually doing and under what conditions.

The last aspect to explain is the role of Hub lists. We can use Hub lists just as a regular table to store and read data. However it's very easy to store data in Hub lists within a flow projects. There's some kind of build-in functionality to store data into Hub lists that are coming from data source or data flows. We will have a look at this in this article. 

## First steps with Hub FLows

![image](/assets/2025-07-07/005.png)

In our first example project we will build and deploy a Hub Flow. The starting for the process to be built is an MQTT data source. It receives a temperature value. This temperture value will be enriched along with a time stamp, just the current time. And this information is stored into a Hub table for long term historization. The temperture sensor that delivers the temperture was already discussed in the article [Peakboard Meets Shelly - Building a Smart Dashboard for Tracking Temperature and Humidity](/Peakboard-Meets-Shelly-Building-a-Smart-Dashboard-for-Tracking-Temperature-and-Humidity.html), so we won't go into all the details to set it up.

The screenshot shows the MQTT data source that is subscribed only to one MQTT node, the temperature.

![image](/assets/2025-07-07/010.png)

We add a data flow along with the data source. The main step is to add one more column. This column is supposed to contain the current time stamp. The second step is only there to change the order of the columns because it looks a little bit nicer when the first column is the time stamp instead of the actual value.

![image](/assets/2025-07-07/020.png)

The last step is to build the Flow. After creating a new Flow we first add a trigger. The Flow should be triggered every time a new MQTT message is sent. So the trigger is the list refresh of our data source.
The first step in the flow is the reload of the Data Flow that adds the time stamp. Other then regular Peakboard Designer projects Data FLows ARE NOT TREIGGERED AUTMATICALLY only because they are bound to a data source. So we need to explicitly trigger the data flow to work. The second step is to store the output of the dataflow into a hub list. There's nothing more to do here, because the list is named and generated automatically from the metadata.

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

