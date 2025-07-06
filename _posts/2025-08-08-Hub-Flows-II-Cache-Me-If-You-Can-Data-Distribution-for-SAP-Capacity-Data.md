---
layout: post
title: Hub Flows II - Cache Me If You Can - Data Distribution for SAP Capacity Data
date: 2023-03-01 00:00:00 +0000
tags: hubflows sap
image: /assets/2025-08-08/title.png
image_header: /assets/2025-08-08/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles about Hub Flows
    url: /category/opcuamqtt
downloads:
  - name: SAPWorkplaceCapacityUpload.pbfx
    url: /assets/2025-08-08/SAPWorkplaceCapacityUpload.pbfx
---
We recently gave an [introduction on Peakboard Hub Flows](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html). In that article, we explained the basics of how to build, deploy, and monitor Peakboard Hub Flows. In this article, we'll take Hub Flows one step further and explain a real-world use case where Hub Flows are very helpful.

First, assume that you have a large number of Peakboard instances running inside your factory or warehouse. Each of these instances pull the same (or similar) data from your ERP system. This could be the data for loading gates (open deliveries) or other data for monitoring processes.

The key point, though, is that each of your Peakboard instances have to individually query your ERP system for the data that they need---even though all the instances need the same data.

You don't want a large number of Peakboard instances, potentially hundreds, to query your ERP system for the same data, every 30 or 60 seconds. This puts a heavy workload on your ERP system.

Instead, you want to query the data once, and then distribute it to all your Peakboard instances---a hub and spoke model. That way, you reduce the workload on your ERP system down to the bare minimum.

So, you create a Hub Flow that queries your ERP system for the necessary data, and stores it in a Hub list. It repeats this process every 90 seconds, so the Hub list always stays up to date. Then, each of your Peakboard instances can get the data they need asynchronously, by pulling it from the Hub list---all without ever having to bother your ERP system.

Now that we've explained the motivation for this example, let's dive into how you actually build it.

## Collect the data

First, we need to collect the data we need, in our ERP system. For this example, lets say that we need workplace capacity data.

The main method for retrieving workplace capacity data is the well-known transaction COOIS. We have an [article on COOIS](/Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html), if you need a refresher. The following screenshots show the selection screen and the output in the SAP GUI. We store our selection (layout and plant) in a variant that we can use later.

![image](/assets/2025-08-08/010.png)

![image](/assets/2025-08-08/020.png)

Let's jump in our Hub Flow project and create a new SAP data source. We use a simple XQL statement "EXECUTE REPORT 'PPIO_ENTRY' USING 'CAPPERWP'" to execute the report "PPIO_ENTRY" with the previously saved variant.

![image](/assets/2025-08-08/030.png)

The execution of the report gives us every operation at every workplace within a given plant. However we only get the workplace name, but not the user friendly, plain description text. To get these description texts we query the view CRHD_V1. It gives us the description texts for a given language for each workplace name.

![image](/assets/2025-08-08/040.png)

The next step will be to join the output of the COOIS transaction with the description texts. So we use a data flow to do so. The fields who serve as join key is the name of the workplace.

![image](/assets/2025-08-08/050.png)

In the next step of the data flow we just remove unaused columns and give the useful columns useful names. That's all.

![image](/assets/2025-08-08/060.png)

## Build the Hub Flow

The actual Hub flow is built to put all the data articfacts together in the right order. The flow is triggered every 90 seconds, Then the capacity value data source is executed, the data source for the plain description texts is executed, the data flow for joining the data is executed and finally the output of the join step is written to a built-in Hub list. We already learned in the first article that it's not necessary to create these lists manually. They are created automatically according of the metadata.

![image](/assets/2025-08-08/070.png)

## Deploy and monitor

After the deployment of the Hub Flow it is executed on regular basis.

![image](/assets/2025-08-08/080.png)

And the hub lists is refreshed accordingly.

![image](/assets/2025-08-08/090.png)

## Consume the data

It's easy to imagine how the connected clients would query the cached data. A regular client at a workplace would just query the data and filter it according to their workplace name.

![image](/assets/2025-08-08/100.png)

We can also imagine how a plant monitoring dashboard would query the data. In that case we don't want to see each single operation, but aggregate all the capacity values and group it by workplace. The next screenshot show the aggregation on top of the raw data that is coming from Hub List.

![image](/assets/2025-08-08/110.png)

As long as the raw data is re-freshed on a regular basis, different kinds of clients can consume and process it. That's the main idea behind of what we built.

## result and conclusion

In this article we learned, how to build a hub and spoke architecture to distribute data from an ERP system to a large number of clients. Such consideration should alway be taken into account when the source system (like SAP) is especially sensitive for repeating workload.
