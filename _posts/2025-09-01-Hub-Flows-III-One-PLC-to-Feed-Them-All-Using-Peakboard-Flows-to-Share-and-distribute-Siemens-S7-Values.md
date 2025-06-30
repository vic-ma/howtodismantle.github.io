---
layout: post
title: Hub Flows III - One PLC to Feed Them All - Using Peakboard Flows to Share and distribute Siemens S7 Values
date: 2023-03-01 00:00:00 +0000
tags: hubflows
image: /assets/2025-09-01/title.png
image_header: /assets/2025-09-01/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles about Hub Flows
    url: /category/opcuamqtt
downloads:
  - name: SiemensS7Distribution.pbfx
    url: /assets/2025-09-01/SiemensS7Distribution.pbfx
  - name: SiemensS7DistributionConsumer.pbmx
    url: /assets/2025-09-01/SiemensS7DistributionConsumer.pbmx
---
Hub FLows are super important when it comes to doing things in the background without user interaction. we already learned the basics about Hub flows in [Getting started and learn how to historize MQTT messages](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html) and also in the second part of the series [Hub Flows II - Cache Me If You Can - Data Distribution for SAP Capacity Data](/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html).

This is the third article in our series to explain and build another typical use case with Hub Flows. Let's imagine we have a Siemens S7 PLC in our factory. The PLC "knows" what a machine is currently doing, if it's running smoothly and if not, what the root cause of the problem is. Let's also imagine we have a large number of other Peakboard clients to consume this information. Because they want to show the current state of the machine on different dashboards in different locations. It's technically not a good idea that all these Peakboard applications connect on their own to the S7. A Siemens PLC does not support a larger number of incoming connections. So what we do now, is that we build a Hub Flow that is the has the only connection to the S7 and gets the necessary data. After having received the data from S7 it writes the values into so called Hub Variables. These Hub Variables are there to store single values of data. The trick is, that the clients (the other Peakboard applications) can subscribe on these variables. So in the moment the Flow writes a new value into the variable all subscribed clients are informed about the change in real-time with being connected directly to the PLC. It's a hub and spoke arhitecture similiar to that we discussed in the [article about caching SAP data](/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html), however this time the clients don't query the cache on a regular basis, but they are subscribed to change by using a Hub variable.

## Preparing the Hub Variable

The first step is to prepare three Hub variables in the Hub portal. We need these three

1. "IsRunning" is a boolean variable representing a value to define if the machine is running
2. "ErrorMessage" contains the error message as a string in case the machine is currenlty in faulty mode
3. "RunningSpeed" is a number representing the speed of the machine if it's running at the moment, otherwise it's 0.

To create a variable we need to define a name and a data type.

![image](/assets/2025-09-01/010.png)

The next screenshot shows our three variables after the creation.

![image](/assets/2025-09-01/020.png)

In the Peakboard designer we create a new Flow project and also create the three variables with the same name and data type. In the second tab of the variable definition dialog we can connect the variable with the one we already prepared in the Hub. This connection will forward all local changes of a variable to the Hub instance simultaniuosly.

![image](/assets/2025-09-01/030.png)

## Preparing the datasource and writing the variables

For the data source we use a classical Siemens S7 data source and configure it to access the three values on the PLC.

![image](/assets/2025-09-01/040.png)

In a simple function named "WriteVariables" we take the values from the S7 data source and write it into the local variables (that are in turn bound to to the Hub variables)

![image](/assets/2025-09-01/050.png)

## Building and deploying the Hub FLow

The Flow is triggered periodically every 10 seconds. It reloads the Siemens S7 data source. After that it executes the function for turning the data source output into variable. Thats all out Hub Flow needs to do.

![image](/assets/2025-09-01/060.png)

After the Deployment the Flow starts it executes right away.

![image](/assets/2025-09-01/070.png)

And the Hub Variable values are filled with the content from Siemens S7.

![image](/assets/2025-09-01/080.png)

## Consuming the data

Let's jump to the consumer project. The only thing we need to do is create a new empty project and set up the same variables variable as we already know with the same binding to the Hub variables. That's all. The content of the variables is subscribed from the Hub value and changes it behaviour in real-time. The screenshot shows a couple of controls to visualize the data: An icon for the running state with condtional formatting, a gauge for the running speed and just the text in case of an error message.

![image](/assets/2025-09-01/090.png)

The animation shows the reaction of the client application when the machine switches to error state and back.

![image](/assets/2025-09-01/result.gif)

## result and conclusion

We learned how to decouple the S7 connectivity from the actual consumer by using Hub FLows and Hub Variables. This allows a massively scalable architecture even when the direct connectivity to the PLC is very limited like with the S7.
Other than our privous example with the SAP caching the actual value trasportation happens in real-time. So the consumer doesn't have to query the data on a regular basis.
