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
1. The Hub Flow stores each value inside its own Hub variable. It overwrites the previous value in the variable. 
    * For example, you might have a boolean Hub variable that reports whether the S7 detects a problem or not.
1. Whenever the Flow gets new data from the S7, it repeats steps 2 and 3.
    * For example, the S7 detects a problem with the machine it's connected to. The Flow sees this in the raw data. So, the Flow changes the `problem_detected` Hub variable from `FALSE` to `TRUE`.

You also need to connect your Peakboard applications to the Flow. Here's what that looks like:
1. If a Peakboard application needs the S7's data, then it subscribes to the Hub variables that the Flow publishes. An application only needs to subscribe to the variables that it needs.
    * For example, if an app needs to know if the S7 detects a problem or not, then it subscribes to the `problem_detected` Hub variable. It does not need to subscribe to other variables, like an energy consumption variable---because it has no need for this data.
1. The Flow writes a new value into a Hub variable.
1. Peakboard Hub notifies all the Peakboard apps that are subscribed to the Hub variable.
1. If an application is notified, it processes the new data, and updates its dashboard accordingly.
    * For example, if it sees that `problem_detected` is now `TRUE`, then it might change a status indicator from green to red. Or it may send an automated email to a manager.

This architecture is commonly known as a [publish-subscribe pattern](https://en.wikipedia.org/wiki/Publish%E2%80%93subscribe_pattern).

Now, let's build an example based on what we've just discussed.

## Prepare the Hub variables

First, let's create three example Hub variables by using the Hub Portal.

| Variable | Data type | Description |
| -------- | --------- | ----------- |
| `IsRunning` | Boolean | Whether the machine is running
| `ErrorMessage` | String | The error message, if the machine encounters a problem. Otherwise, it's the empty string.
| `RunningSpeed` | Number | The speed that the machine is running at, if it's running. Otherwise it's 0.

![image](/assets/2025-09-01/010.png)

Here's what our three variables look like after creation:

![image](/assets/2025-09-01/020.png)

## Create the Flow

In Peakboard Designer, we create a new Flow project.

We create three variables to match the Hub variables. We use the same names and data types.

We also connect each variable to their respective Hub variables:
1. Edit the variable.
1. Go to the *Peakboard Hub* tab
1. Under *Peakboard Hub Variables*, select the corresponding Hub variable.

This makes it so that the Hub variables automatically updates to the values in our local variables.

![image](/assets/2025-09-01/030.png)

### Prepare the data source

Next, we create a data source for the S7 data. We use the standard Siemens S7 data source.

We configure it to give it access to the three variables on the S7:

![image](/assets/2025-09-01/040.png)

### Write the variables

Next, we create a simple function that takes the values from the S7 data source and writes the data into local variables (these are bound to to the Hub variables, ).

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

We learned how to decouple the S7 connectivity from the actual consumer by using Hub Flows and Hub Variables. This allows a massively scalable architecture even when the direct connectivity to the PLC is very limited like with the S7.
Other than our privous example with the SAP caching the actual value trasportation happens in real-time. So the consumer doesn't have to query the data on a regular basis.
