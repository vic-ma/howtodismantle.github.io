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
Azure Event Hubs are a high-throughput, real-time data ingestion service. You can think of it as a large, scalable entry-point for streamed data (e.g. telemetry, logs, sensor data, clickstreams, IoT signals). In a factory environment, you'll often see Event Hubs being used to ingest telemetry and sensor data from machines. To learn the basics of Event Hubs and understand how data flows from the source to the destination, check out Microsoft's [introduction to Event Hubs](https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-about).

There are two main ways to integrate Azure Event Hubs into Peakboard:
* Peakboard acts as an **event source** and streams events to Azure Event Hubs.
* Peakboard acts as an **event consumer.** Peakboard subscribes to specific events. Whenever Azure Event Hubs receives those events from its publishers, it sends the events to Peakboard.

In this article, we'll take a look at both of these scenarios and explain how they work.

## Configure Event Hubs namespace

First, we configure our Event Hubs namespace. An [Event Hubs namespace](https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-features#namespace) is a collection of one or more Event Hub instances. The namespace contains the actual Event Hub instance that we will use use. Here's what our `DismantleEvents` namespace looks like:

![Azure Event Hub namespace overview](/assets/2026-01-06/azure-event-hub-namespace-overview.png)

We need to create an access policy, in order to authorize our Peakboard app to connect to the Event Hub. From the sidebar, we go to *Settings > Shared access policies.* We create a new access policy and enable the *Manage* permission. Later, our app will need the connection string.
![image](/assets/2026-01-06/azure-event-hub-access-policy.png)

## Configure Storage account

Next, we configure our [Azure Storage account](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-overview). The storage account stores the [stream offsets](https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-features#stream-offsets) for our Event Hub's partitions. These offsets keep track of where the newest event is located, for each partition.

Again, we need to get the connection string, which our Peakboard app will need later. To get this information, we go to *Sidebar > Security + networking > Access keys*.

![Azure storage account connection string](/assets/2026-01-06/azure-storage-account-connection-string.png)

## Create the Event Hub data source

Now, let's go to Peakboard Designer and create an Event Hub data source. Whether you want to create a Peakboard app or a Hub Flow, the process for setting up the Event Hub data source looks the same. For this article, we're going to create a Peakboard app.

We add a new Azure Event Hub data source. Then, we enter the connection strings for both the Event Hub and the storage account. We also enter the Event Hub name and the storage account name.

![Peakboard Azure Event Hub data source settings](/assets/2026-01-06/peakboard-azure-event-hub-data-source-settings.png)

## Create an event consumer

Now, let's create an event consumer, in the form of a Hub Flow.

The idea of our Flow is to build a bridge between a machine and the Event Hub, so we also need the connection to the machine. That part is simple with an OPC UA data source. We subscribe to two OPC UA nodes that each represent a counter of a light barrier. They count the goods that pass through the light barrier, so our data source has two columns—one for each light barrier—that contain the counters.

![Peakboard OPC UA light barrier data source](/assets/2026-01-06/peakboard-opc-ua-light-barrier-data-source.png)

The next screenshot shows the actual flow. It is triggered by the OPC UA data source. Every time one of our two light barriers sends an updated value the flow is triggered, and every time it is triggered it calls the function `SendToAzure`.

![Peakboard Flow SendToAzure overview](/assets/2026-01-06/peakboard-flow-sendtoazure-overview.png)

Here's the last step we need to get it working. The `SendToAzure` function just calls the function `sendevent` of our Azure Event Hub data source. It takes two parameters. The first parameter is the actual message. In our case we build a JSON string to encapsulate the two light barrier counter values. The second parameter is a set of properties that can be seen as metadata bound to the actual message. These properties can be used with the Event Hub in Azure for message routing or other logic. For our example we do not use them, so the code in the screenshot is only for demonstration purposes to show how to use it.

![Peakboard SendToAzure function script](/assets/2026-01-06/peakboard-sendtoazure-function-script.png)

After deploying the Flow on a Peakboard Hub server it will work right away and send all subscribed light barrier values to the Azure Event Hub without any additional adjustments.

## Create an event publisher

To set up a consumer application that receives and processes subscribed messages from Azure Event Hub we need a data source similar to the one we used for building the flow. The output of the data source is a table with two columns: Timestamp and Message. The maximum number of rows is determined through the parameter `Queue Size`. The data can be processed by using the common patterns like the Reload Event or data flow.
As an alternative we can use a special event that is fired for every arriving message. The logic is built with Building Blocks within this event. In our example we just parse the incoming message and assign the values of the light barriers to two text blocks to show the value.
This approach keeps the consumer logic adaptable and ready for future enhancements, such as reacting to additional properties in the event payload.

![Peakboard Event Hub message consumer script](/assets/2026-01-06/peakboard-event-hub-message-consumer-script.png)

## Result

The image shows our example in running mode. The messages are generated from OPC UA, sent to Event Hub, and then forwarded to the application. The application shows the raw data in the table (just a table control bound to the data source) and also the two text blocks that show the processed values from the incoming messages. It also highlights how quickly production data can flow through the entire architecture once the connections are configured correctly.

![Peakboard Azure Event Hub integration final result](/assets/2026-01-06/result.gif)
