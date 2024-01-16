---
layout: post
title: Integrating Node-RED and Peakboard - Part I - Real-time calculator
date: 2023-03-01 12:00:00 +0200
tags: api opcuamqtt nodered
image: /assets/2024-03-23/title.png
read_more_links:
  - name: How to install node-red on Windows
    url: https://nodered.org/docs/getting-started/windows
  - name: Node-RED main website
    url: https://nodered.org/
  - name: Part I - Real-time calculator
    url: /Integrating-Node-RED-and-Peakboard-Part-I-Real-time-calculator.html
  - name: Part II - Sending Alerts to a Peakboard application
    url: /Integrating-Node-RED-and-Peakboard-Part-II-Sending-Alerts-to-an-Peakboard-application.html
downloads:
  - name: NodeRedCalculator.pbmx
    url: /assets/2024-03-23/NodeRedCalculator.pbmx
  - name: Node-RED flow used in this article
    url: /assets/2024-03-23/flow.json
---
In this two-part article series, we will discuss the best practices for integrating Peakboard with node-RED applications. Most of the time, communication between node-RED and the outside world happens through MQTT or HTTP. We will use the HTTP option:

[Part I - Real-time calculator](/Integrating-Node-RED-and-Peakboard-Part-I-Real-time-calculator.html)
Shows how to submit information from Peakboard to node-RED, do something with it and then process the result in real-time

[Part II - Sending Alerts to a Peakboard application](/Integrating-Node-RED-and-Peakboard-Part-II-Sending-Alerts-to-an-Peakboard-application.html)
Shows how to submit an alert from a node-RED flow to visualizize in a Peakboard application.

## What we build here

The showcase of this article will be, that the end user can use a Peakboard app to type in two numbers. Then the two values are submitted to a node-RED flow to be summarized then return the result. We're doing this exercise to understand how to submit and receive information from node-RED while the process is triggered from the Peakboard side.

## Building the node-RED flow

Here's the flow we're using. It starts with a "http in" node to receive the call. Then we add the two numbers, which are part of the JSon payload. The next node is a template node to create the answer and finally we return the answer to the caller by using a "http response" node. To the relevant nodes we also added debug nodes. So we can follow the flow in the debug window when it's executed.

![image](/assets/2024-03-23/010.png)

The "http in" node listens at the "/Add" URL:

![image](/assets/2024-03-23/020.png)

The function node adds the two numbers (X and Y) and creates a third attribute i the payload for the result:

![image](/assets/2024-03-23/021.png)

The actual response is created with a template node. It's just simple json with a placeholder pointing to the result we created in the last step:

![image](/assets/2024-03-23/022.png)

The "http response" doesn't need any additional configuration. It just returns the payload to the initial http caller:

![image](/assets/2024-03-23/023.png)

## Building the Peakboard app

The Peakboard app is super simple. There are only two text fields for the input, a button and a text field for the output. We don't forget to give the textfields proper names to use them in scripting.

![image](/assets/2024-03-23/030.png)

The actual script consists only of two steps. In the first step the http endpoint of the node-RED flow is called. We concatenate the strings to build a proper JSon with the two numbers X and Y coming from the text fields. Here's how the JSon should look like:

{% highlight json %}
{
    "X": 36,
    "Y": 42
}
{% endhighlight %}

We also must add the header for "Content-Type" to "application/json". Otherwise the flow doesn't understand that we're sending a JSon object.
The call returns the result JSon. Here's how it looks like:

{% highlight json %}
{
    "Result": 78
}
{% endhighlight %}

To extract the result value from the response we use a JSon path block and address the Result attribute. The value is put into the text field we prepapred earlier.

![image](/assets/2024-03-23/040.png)

## result and conclusion

Here's the application in action and also the output of the debug window in node-RED to follow the steps and the calculcation in real time.

![image](/assets/2024-03-23/result.gif)

![image](/assets/2024-03-23/050.png)

