---
layout: post
title: Integrating Node-RED and Peakboard - Part II - Sending Alerts to an Peakboard application
date: 2023-03-01 12:00:00 +0200
tags: api opcuamqtt nodered
image: /assets/2024-04-08/title.png
read_more_links:
  - name: How to install node-red on Windows
    url: https://nodered.org/docs/getting-started/windows
  - name: Node-RED main website
    url: https://nodered.org/
  - name: Part I - Real-time calculator
    url: /Integrating-Node-RED-and-Peakboard-Part-I-Real-time-calculator.html
  - name: Part II - Sending Alerts to an Peakboard application
    url: /Integrating-Node-RED-and-Peakboard-Part-II-Sending-Alerts-to-an-Peakboard-application.html
downloads:
  - name: NodeRedAlert.pbmx
    url: /assets/2024-04-08/NodeRedAlert.pbmx
  - name: Node-RED flow used in this article
    url: /assets/2024-04-08/flow.json
---
In this two-part article series, we discuss some best practices for integrating Peakboard with Node-RED applications.

Most of the time, communication between Node-RED and the outside world happens through MQTT or HTTP. We will use the HTTP option:

[Part I - Real-time calculator](/Integrating-Node-RED-and-Peakboard-Part-I-Real-time-calculator.html) -
How to send information from Peakboard to Node-RED, do something with it, and then process the result in real-time.

[Part II - Sending Alerts to a Peakboard application](/Integrating-Node-RED-and-Peakboard-Part-II-Sending-Alerts-to-an-Peakboard-application.html) -
How to send an alert from a Node-RED flow and visualize it in a Peakboard application.

## What we build here

The showcase we're building here is to demonstrate how to send a message from node-RED to a Peakboard box. This is commonly used to inform a Peakboard user about a certain event that is happening in the node-RED flow. So in the example we just send an alert. The alert consists of a message and a Priority field. The message is shown in the Peakboard application and if the Priority is 'A' the background is turning red.

## Building the Peakboard app

The first thing we need in the Peakboard app is a function called SubmitAlert. We're making it 'shared'. So the function can not only be called from within the app but also from the outside by calling a http endpoint on the box. The function has two parameters: Priority and Message.

![image](/assets/2024-04-08/010.png)

The canvas is pretty simple. The main element is just a text field with a certain name, so we can use later it in the script.

![image](/assets/2024-04-08/020.png)

Let's jump into the function. The two parameters are available in the block tree on the right side. The functionality we're building is quite simple. When the Priority value is 'A' we make the background red, otherwise we make it transparent. The Message value is just assigned to the text field and a "Alert Processed" message is returned to the caller. That's it.

![image](/assets/2024-04-08/030.png)

## Preparing the box

Later we will call the http endpoint of the shared function on the box. Of course this endpoint is protected with username and password to prevent unwanted people from calling it. But on the other side we don't want to put the box's admin credentials into the node-RED flow. That's why we add an additional user with very limited rights to the box. This can be done in the "User and Roles" section of the box admnistration.

![image](/assets/2024-04-08/035.png)

## Building the flow

The screenshot below shows the node-RED flow. It starts with a manual starting point to trigger the flow (just be clicking on it becaue it's just a demo). The actualy JSon body is created in a Template node and then sent via a "http request" node. To monitor this we put some debug nodes on the side, so we can follow the call when the flow is executed.

![image](/assets/2024-04-08/040.png)

Let's dive into the template node. The JSon built here follows the need to submit parameters to a shared function with the two parameters discussed earlier.

![image](/assets/2024-04-08/050.png)

The tricky part is the http request node. First we need to fill in the URL to the box endpoint for the shared function. Then we need to fill the authentification credentials for the user we created earlier. The last thing is to add a header for the "Content-Type" and set it to "application/json" to make sure the webserver of the box knows what to do with it.

![image](/assets/2024-04-08/060.png)

## Action!

Here's how the flow looks like when executed. We check the log messages on the right side. It shows the JSon and more important it shows what is returned from the call to the box. The term "Alert processed" is created by the Peakboard shared function.

![image](/assets/2024-04-08/070.png)

Here's the result in the Peakboard app. The alert is shown and the background is set to red because of the A priority.

![image](/assets/2024-04-08/050.png)
