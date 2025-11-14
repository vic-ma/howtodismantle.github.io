---
layout: post
title: Stream Dream - Bringing Azure Event Hub Data to Life in Peakboard
date: 2023-03-01 03:00:00 +0200
tags: hubflows 
image: /assets/2026-01-06/title.png
image_landscape: /assets/2026-01-06/title.png
bg_alternative: true
read_more_links:
  - name: Azure Event Hub Overview
    url: https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-about
downloads:
  - name: AzureEventHub.pbmx
    url: /assets/2026-01-06/AzureEventHub.pbmx
  - name: OPCUAToAzureHub.pbfx
    url: /assets/2026-01-06/OPCUAToAzureHub.pbfx
---
Azure Event Hubs are a high-throughput, real-time data ingestion service. We can think of it as a big, scalable entry point for streaming data — telemetry, logs, sensor data, clickstreams, IoT signals, etc. A typical use case in factory environments would be telemetry or sensor data from machines. [This article](https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-about) describes the basic concepts and how typically the data is flowing from what to where in a very good way.

For Peakboard application there are basically too touchpoints:

1. Peakboard can act as source of events and stream these events to the Azure Event Hub
2. Peakboard can also be a consumer of events and can react on incoming events it has subscribed to

We will discuss both directions in this article.

## Set up the Azure environment

On the Azure side the set up is fairly easy. The frame for hanndling events is the Event Hubs Namespace. It bundles one or more Event Hubs. The next screenshot shows the namespace.   

![image](/assets/2026-01-06/010.png)

And below the Namespace is the one and only Event Hub. Within the Event Hub we will need an access policy. From the access policy we get the connection string that we will need later to configre the data source on the Peakboard side.

![image](/assets/2026-01-06/020.png)

The last Azure object is a Storage Account. It is needes and used  to store the offset points for the message streaming. The csreenhot show how to get the connection string. We will need it later.

![image](/assets/2026-01-06/030.png)

## Set up the Peakboard Flow for Sending messages.

On the Peakboard side - regardless if we use it in Flow or a regular design project - We must provide the connection string for both the Event Hub and Storage account along with the Hub name and the storgae account name.

![image](/assets/2026-01-06/040.png)

The idea of our Flow is to build a bridge between a machine and the Event Hub. So we also need the connection to the machine. It's very simple by using an OPC UA data source. We will subscribe to two OPC UA nodes that each repesents a counter of a light barrier. It's just counting the goods that pass through the light barrier. So our data source has two columns for the two light barriers that contain the counters.

![image](/assets/2026-01-06/050.png)

The next screenshot shows the actual flow. It's triggered by the OPC UA data source. So every time one of out two light barriers sends an updated value the flow is triggered. And very time it's triggered it calls the function `SendToAzure`.

![image](/assets/2026-01-06/060.png)

Here's the last step we need to get it working. The `SendToAzure` funtion just calls the function `sendevent` of out Azure Data Hub source. It takes two parameters. The first parameter is the actual message. In our case we just build a JSON string to encapsulate the two light barrier counter values. The second parameter is a set of properties that can be seend as some kind of meta information that is bound to the actual message. These properties can be used with the Event Hub in Azure for message routing or other logic. For out example we don't use these. So the code in the screenshot is only for demonstration purpose on how to use it.

![image](/assets/2026-01-06/070.png)

After having the Flow deployed on a Peakboard Hub server it will work right away and send all subscribed light barrier values to the Azure Event Hub.

## Set up a Message Consumer

To set up a consumer application that receives and processes subscribed messages from Azure Event Hub we need a data source similiar to that  we used for the building the flow. The actual output of the data source is a table with two columns: Timestamp and Message. The maximum number of rows is determined through the parameter `Queue Size`. It can processed by using the common patterns like the Reload Event or data flow. 
As an alternative we can use a special event that is fired for every arriving message. The logic that is built by Building Blocks within this event. In our example we just parse the incoming message and assign the values of the light barriers to two text blocks to show the value.

![image](/assets/2026-01-06/070.png)

## Result

The image shows our example in running mode. The messages are generated from OPC UA and sent to Event Hub and then forwarded to the application. The application shows the raw data in the table (just a tbale control bound to the data source) and also the two text blocks that show the processed values from the incocming messages....

![image](/assets/2026-01-06/result.gif)
