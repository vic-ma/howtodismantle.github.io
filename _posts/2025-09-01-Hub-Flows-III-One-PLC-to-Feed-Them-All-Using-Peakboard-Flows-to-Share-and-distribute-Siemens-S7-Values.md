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
Hub Flows let you run automated tasks the background without any user interaction. We covered the basics of Hub Flows in [part I](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html) and [part II](/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html) of our Hub Flows series. In this article, we'll take a look at another typical use case with Hub Flows.

## The scenario

Imagine you have a Siemens S7 PLC in your factory. The S7 "knows" what a machine is currently doing. It knows if a machine is running smoothly, and if there's a problem, it knows what root cause is.

Now, imagine that you have a large number of Peakboard applications that need the information from the S7. For example, you have Peakboard Boxes at different places in your factory, with different dashboards, and they all use the same information from the S7.

It's not a good idea to have all these Peakboard apps connect to the S7 on their own. Siemens PLCs do not support a large number of incoming connections.

## The proper solution

Instead, you should use a Hub Flow. Here's how the Hub Flow works:
1. The Hub Flow connects to the S7 and retrieves the necessary data. None of the Peakboard apps connect to the S7, so there's only one connection that the S7 has to handle.
1. The Hub Flow processes the raw data in order to get a number of useable values.
    * For example, whether the S7 detects a problem or not.
1. The Hub Flow stores each value inside its own Hub Variable. It overwrites the previous value in the Variable. 
    * For example, you might have a boolean Hub Variable that reports whether the S7 detects a problem or not.
1. The Hub Flow automatically runs every 10 seconds, repeating steps 1 to 3.
    * For example, 10 seconds passes and our Hub Flow requests new data from the S7. The Hub Flow processes the raw data and and updates the Hub Variables accordingly. For example, if the new S7 data says that the machine has a problem, then the Flow would change the `problem_detected` Hub Variable from `FALSE` to `TRUE`.

You also need to connect your Peakboard applications to the Flow. Here's what that looks like:
1. If a Peakboard application needs the S7's data, then it subscribes to the Hub Variables that the Flow publishes. An application only needs to subscribe to the Variables that it needs.
    * For example, if an app needs to know if the S7 detects a problem or not, then it subscribes to the `problem_detected` Hub Variable. It does not need to subscribe to other Variables, like an energy consumption Variable---because it has no need for this data.
1. The Flow writes a new value into a Hub Variable.
1. Peakboard Hub notifies all the Peakboard apps that are subscribed to the Hub Variable.
1. If an application is notified, it processes the new data, and updates its dashboard accordingly.
    * For example, if it sees that `problem_detected` is now `TRUE`, then it might change a status indicator from green to red. Or it may send an automated email to a manager.

This architecture is commonly known as a [publish-subscribe pattern](https://en.wikipedia.org/wiki/Publish%E2%80%93subscribe_pattern).

Now, let's build an example based on what we've just discussed.

## Prepare the Hub Variables

First, let's create three example Hub Variables by using the Hub Portal.

| Variable | Data type | Description |
| -------- | --------- | ----------- |
| `IsRunning` | Boolean | Whether the machine is running
| `RunningSpeed` | Number | The speed that the machine is running at, if it's running. Otherwise it's 0.
| `ErrorMessage` | String | The error message, if the machine encounters a problem. Otherwise, it's the empty string.

![image](/assets/2025-09-01/010.png)

Here's what our three Variables look like after creation:

![image](/assets/2025-09-01/020.png)

## Create the Flow

In Peakboard Designer, we create a new Flow project.

We create three variables to match the Hub Variables. We use the same names and data types.

We also connect each variable to their respective Hub Variables:
1. Edit the variable.
1. Go to the *Peakboard Hub* tab
1. Under *Peakboard Hub Variables*, select the corresponding Hub Variable.

This makes it so that the Hub Variables automatically updates to the values in our local variables.

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

We showed how to decouple an S7 and Peakboard apps, by using Hub Flows and Hub Variables. This allows a massively scalable architecture even when the direct connectivity to the PLC is very limited like with the S7.
Other than our privous example with the SAP caching the actual value trasportation happens in real-time. So the consumer doesn't have to query the data on a regular basis.
