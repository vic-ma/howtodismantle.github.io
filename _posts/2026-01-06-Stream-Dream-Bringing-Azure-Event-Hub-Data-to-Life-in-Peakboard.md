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
Azure Event Hubs are a high-throughput, real-time data ingestion service. You can think of it as a large, scalable entry-point for streamed data (e.g. telemetry, logs, sensor data, clickstreams, IoT signals). In a factory environment, you might see Event Hubs being used to ingest telemetry and sensor data from machines. To learn the basics of Event Hubs and understand how data flows from the source to the destination, check out Microsoft's [introduction to Event Hubs](https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-about).

There are two ways to integrate Event Hubs into Peakboard:
* Peakboard acts as an event source and streams these events to Azure Event Hub.
* Peakboard acts as an event consumer. Azure Event Hub sends events to Peakboard, based on the consumer group

We will discuss both directions in this article.

## Set up the Azure environment

On the Azure side the setup is fairly easy. The framework for handling events is the Event Hubs namespace, which bundles one or more Event Hubs. The next screenshot shows the namespace.

![image](/assets/2026-01-06/azure-event-hub-namespace-overview.png)

Within that namespace sits the Event Hub we use. We also need an access policy for the Event Hub. The access policy provides the connection string that we will need later to configure the data source on the Peakboard side.

![image](/assets/2026-01-06/azure-event-hub-access-policy.png)

The last Azure object is a Storage Account. It is needed to store the offset points for the message streaming. The screenshot shows how to get the connection string, which we will need later.

![image](/assets/2026-01-06/azure-storage-account-connection-string.png)

## Set up the Peakboard Flow for sending messages

On the Peakboard side—regardless of whether we use Flow or a regular design project—we must provide the connection string for both the Event Hub and the Storage Account along with the hub name and the storage account name.

![image](/assets/2026-01-06/peakboard-azure-event-hub-data-source-settings.png)

The idea of our Flow is to build a bridge between a machine and the Event Hub, so we also need the connection to the machine. That part is simple with an OPC UA data source. We subscribe to two OPC UA nodes that each represent a counter of a light barrier. They count the goods that pass through the light barrier, so our data source has two columns—one for each light barrier—that contain the counters.

![image](/assets/2026-01-06/peakboard-opc-ua-light-barrier-data-source.png)

The next screenshot shows the actual flow. It is triggered by the OPC UA data source. Every time one of our two light barriers sends an updated value the flow is triggered, and every time it is triggered it calls the function `SendToAzure`.

![image](/assets/2026-01-06/peakboard-flow-sendtoazure-overview.png)

Here's the last step we need to get it working. The `SendToAzure` function just calls the function `sendevent` of our Azure Event Hub data source. It takes two parameters. The first parameter is the actual message. In our case we build a JSON string to encapsulate the two light barrier counter values. The second parameter is a set of properties that can be seen as metadata bound to the actual message. These properties can be used with the Event Hub in Azure for message routing or other logic. For our example we do not use them, so the code in the screenshot is only for demonstration purposes to show how to use it.

![image](/assets/2026-01-06/peakboard-sendtoazure-function-script.png)

After deploying the Flow on a Peakboard Hub server it will work right away and send all subscribed light barrier values to the Azure Event Hub without any additional adjustments.

## Set up a message consumer

To set up a consumer application that receives and processes subscribed messages from Azure Event Hub we need a data source similar to the one we used for building the flow. The output of the data source is a table with two columns: Timestamp and Message. The maximum number of rows is determined through the parameter `Queue Size`. The data can be processed by using the common patterns like the Reload Event or data flow.
As an alternative we can use a special event that is fired for every arriving message. The logic is built with Building Blocks within this event. In our example we just parse the incoming message and assign the values of the light barriers to two text blocks to show the value.
This approach keeps the consumer logic adaptable and ready for future enhancements, such as reacting to additional properties in the event payload.

![image](/assets/2026-01-06/peakboard-event-hub-message-consumer-script.png)

## Result

The image shows our example in running mode. The messages are generated from OPC UA, sent to Event Hub, and then forwarded to the application. The application shows the raw data in the table (just a table control bound to the data source) and also the two text blocks that show the processed values from the incoming messages. It also highlights how quickly production data can flow through the entire architecture once the connections are configured correctly.

![image](/assets/2026-01-06/result.gif)
