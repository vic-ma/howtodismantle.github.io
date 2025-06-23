---
layout: post
title: Hub Flows II - Cache Me If You Can - Data Distribution for SAP Capacity Data
date: 2023-03-01 00:00:00 +0000
tags: hubflows
image: /assets/2025-08-08/title.png
image_header: /assets/2025-08-08/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles around Hub FLows
    url: /category/opcuamqtt
downloads:
  - name: SAPWorkplaceCapacityUpload.pbfx
    url: /assets/2025-08-08/SAPWorkplaceCapacityUpload.pbfx
---
We recently discussed the first steps with the brand new Peakboard Hub FLows: [Getting started and learn how to historize MQTT messages](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html). In this first article we learned how to build, deploy and monitor Peakboard Hub Flows.

In today's article will take Hub FLows one step further and discuss a typical use case in which Hub Flows are very helpful. Let's a assume we have a large number of Peakboard instances running within the factory or warehouse. All these clients use the same or similiar information, that is queried from an ERP system. This could be the data for loading gates (open deliveries) or other data to monitor processes and build dashbaords. We don't want a large number of these clients, potentially hundreds, to query the same or at least similiar data very 30 or 60 seconds. This would be generate heavy, yet avoidable workload on the ERP systems. Instead we want to query the data once and then distribute it to all connected clients - a hub and spoke architecure. The workload for the ERP system is reduced to minimum.

So in our example we assume to have many different workplaces, with a Peakboard application running at each workplace. In one central Hub Flow the workplace capacity data is queried from the SAP system and stored in a caching table to which every client can get access to the data asnychronously without bothering SAP. This central cache is supposed to be refreshed every 90 seconds and it contains all operations that are supposed to be processed by any of the connected workplaces.

## Collecting the data

The main basis for the workplace capacity is the well-known transaction COOIS. We already discussed it early 2024 in [this article](/Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html). The screenshots show the selection screen and the output in the SAP Gui. We store our selection (layout and plant) in a variant that we can use later.

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
