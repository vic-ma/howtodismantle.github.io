---
layout: post
title: Hub Flows III - One PLC to Feed Them All - Using Peakboard Flows to share and distribute Siemens S7 values
date: 2023-03-01 00:00:00 +0000
tags: hubflows
image: /assets/2025-09-01/title.png
image_header: /assets/2025-09-01/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles about Hub Flows
    url: /category/hubflows
downloads:
  - name: SiemensS7Distribution.pbfx
    url: /assets/2025-09-01/SiemensS7Distribution.pbfx
  - name: SiemensS7Consumer.pbmx
    url: /assets/2025-09-01/SiemensS7Consumer.pbmx
---
Hub Flows let you run automated tasks the background, without any user interaction. We covered the basics of Hub Flows in [Part I](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html) and [Part II](/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html) of our Hub Flows series. In this article, we'll take a look at another typical use case with Hub Flows.

## The scenario

Imagine you have a Siemens S7 PLC in your factory. The S7 "knows" what a machine is currently doing. It knows if a machine is running smoothly, and if there's a problem, it knows what root cause is.

Now, imagine that you have a large number of Peakboard applications that need the information from the S7. For example, you have Peakboard Boxes at different places in your factory, with different dashboards, and they all use the same information from the S7.

It's not a good idea to have all these Peakboard apps connect to the S7 on their own, because it might overload the S7. Siemens PLCs do not support a large number of incoming connections.

## The proper solution

Instead, you should create a Hub Flow. It works like this:
1. The Hub Flow connects to the S7 and retrieves the necessary data. None of the Peakboard apps connect to the S7, so there's only one connection that the S7 has to handle.
1. The Hub Flow processes the raw data in order to get the values it needs.
    * For example, the Hub Flow might look through the data to find the running speed value.
1. The Hub Flow stores each value inside its own Hub Variable. It overwrites any previous value in the Hub Variable. 
    * For example, you might have a boolean Hub Variable that reports whether the S7 detects a problem or not.

The Hub Flow automatically runs every 10 seconds. Whenever it runs, it repeats steps 1 to 3.

### The Peakboard app

You also need to connect your Peakboard applications to the Hub variables. Here's what that looks like:
1. If a Peakboard application needs the S7's data, then it subscribes to the Hub Variables that the Flow publishes. An application only subscribes to the Variables that it needs.
    * For example, if an app needs to know if the S7 detects a problem or not, then it subscribes to the `problem_detected` Hub Variable. It does not need to subscribe to other Variables, like an `energy_consumption` Variable---because it has no need for that data.
1. The Flow writes a new value into a Hub Variable.
1. Peakboard Hub notifies all the Peakboard apps that are subscribed to the Hub Variable.
1. If an application is notified, it processes the new data, and updates its dashboard accordingly.
    * For example, if an app sees that `problem_detected` is now `TRUE`, then it might change a status indicator from green to red. Or it may send an automated email to a manager.

This architecture is based on a [publish-subscribe pattern](https://en.wikipedia.org/wiki/Publish%E2%80%93subscribe_pattern).

Now, let's build an example based on what we've just discussed.

## Prepare the Hub Variables

First, we create three Hub Variables in the Hub Portal.

| Hub Variable | Data type | Description |
| -------- | --------- | ----------- |
| `IsRunning` | Boolean | Whether the connected machine is running.
| `RunningSpeed` | Number | The speed that the machine is running at, if it's running. Otherwise it's 0.
| `ErrorMessage` | String | The error message, if the machine encounters a problem. Otherwise, it's an empty string.

![image](/assets/2025-09-01/010.png)

Here's what our three Variables look like after creation:

![image](/assets/2025-09-01/020.png)

## Create the Flow

In Peakboard Designer, we create a new Flow project.

We create three local variables that match the Hub Variables. We use the same names and data types.

We also connect each variable to their respective Hub Variables:
1. Edit the variable.
1. Go to the *Peakboard Hub* tab
1. Under *Peakboard Hub Variables*, select the corresponding Hub Variable.

Whenever our local variables updates, the Flow automatically updates the Hub Variables.

![image](/assets/2025-09-01/030.png)

### Prepare the data source

Next, we create a data source for the S7 data. We use the standard Siemens S7 data source.

We configure it to give it access to the three variables on the S7:

![image](/assets/2025-09-01/040.png)

### Write the variables

Next, we create a simple function that takes the values from the S7 data source and writes the data into local variables (remember, these automatically update Hub Variables).

![image](/assets/2025-09-01/050.png)

## Build and deploy the Hub Flow

The Flow automatically runs every 10 seconds. Every 10 seconds, it does the following:
1. Reload the Siemens S7 data source, in order to get new data from the S7.
1. Execute the function that processes the raw S7 data and updates the local variables. (This automatically updates the Hub Variables.)

![image](/assets/2025-09-01/060.png)

We deploy the Hub Flow and it starts working right away:

![image](/assets/2025-09-01/070.png)

You can see that the Flow updates the Hub Variables with the data from the S7:

![image](/assets/2025-09-01/080.png)

## Consume the data

Now, let's create a Peakboard app that pulls data from this Hub Flow.

We create three variables and bind them to the three Hub Variables. These variables work like normal variables, except that they update automatically whenever the corresponding Hub Variables change.

Next, we add a couple of controls to visualize the three variables:
* An icon control for `IsRunning`, built with conditional formatting.
* A gauge control for `RunningSpeed`.
* A text control for `ErrorMessage`, in case one shows up.

![image](/assets/2025-09-01/090.png)

This video shows what our app looks like when the S7 detects an error:

![image](/assets/2025-09-01/result.gif)

## Conclusion

We showed how you can decouple a PLC from the Peakboard apps that need its data. By using Hub Flows and Hub Variables, you can build a highly scalable architecture---even when direct connectivity to the PLC is very limited, like with the S7.

Unlike our previous example with the [SAP caching](/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html), the data transfer (of the variables) happens in real-time with this example. This means that the Peakboard apps don't have to query the data on a regular basis.
