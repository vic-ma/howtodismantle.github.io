---
layout: post
title: DIY Guide - Transform Your Peakboard Box into an MQTT Server 
date: 2023-03-01 00:00:00 +0000
tags: opcuamqtt
image: /assets/2025-02-21/title.png
image_header: /assets/2025-02-21/title_landscape.png
read_more_links:
  - name: MQTT Server Extension
    url: https://templates.peakboard.com/extensions/MQTT-Server/index
  - name: Other MQTT topics
    url: /category/opcuamqtt
downloads:
  - name: MQTTDIY.pbmx
    url: /assets/2025-02-21/MQTTDIY.pbmx
---
We've already discussed many projects that use MQTT to communicate between the Peakboard application and all kinds of sensors or other hardware and software entities. MQTT is a super easy and nice option to couple all these entities together. Especially because there's no need for the entities to "know" each other. They just submit their messages to an MQTT broker or subscribe to messages from the broker.

The downside of MQTT is that we need an MQTT broker. So even when there's only one sensor and one Peakboard application, we need an additional server to exchange the MQTT messages.

In this article, we will discuss a nice and easy way to turn a Peakboard Box into a fully functional MQTT broker. So in a simple scenario with one sensor and one Peakboard application, no additional server is necessary. The sensors connect directly to a Peakboard Box that exposes an MQTT broker endpoint. So we can avoid the set-up of a separate MQTT server.

If scenarios get more complex (e.g. 5 sensors and multiple boxes with bidirectional message exchange), we must carefully evaluate if this technique still makes sense. For a good, robust architecture, it might be better to switch back to the traditional, separate MQTT server, and not rely on a single Box.

## Install and use the the MQTT Server extension

To set up an MQTT broker along with the Peakboard application we use MQTT Server extension that can be directly installed from the extension pane when creating a new data source.

![image](/assets/2025-02-21/010.png)

The MQTT server only needs to know the port to listen for incoming connections, by default it's 1883. To complete the configuration we must hit the reload button. The output of the data source can be used to display the current state of the sever: "Init" or "Running".

![image](/assets/2025-02-21/020.png)

The server needs a start command to open the endpoint for external connections. That's why we set up a timer that only runs once. Within this timer event we call the "Start" function of the MQTT Server data source.

![image](/assets/2025-02-21/030.png)

One more things to consider is letting this MQTT server run on a Peakboard box (or on a BYOD instance). It opens a TCP/IP port to the outside world and so triggers the Windows firewall to prevent this. So using the MQTT server on a box for this first time needs to confirm this pop up dialog and let it open the port in the Windows firewall. 

![image](/assets/2025-02-21/040.png)

## Configure a button to send MQTT messages

As an MQTT enabled sensors we're using a Shelly button. In [this article](/Building-an-emergency-button-with-Shelly-Button1-and-MQTT.html) we already discussed how to use and set up this kind of button. Every time the button is clicked a JSON string is sent to to dedicated MQTT broker. In our example the MQTT broker is the Peakboard box. So we put the ip address of the box in the corresponding field of the Shelly button web configuration.

![image](/assets/2025-02-21/050.png)

## Building the Peakboard app

For the actual MQTT source we choose "localhost" as the MQTT broker address. So when the Peakboard application is running the broker can be always reached under "localhost". Furthermore we subscribe to the MQTT node the Shelly button is submitting its events. 

![image](/assets/2025-02-21/060.png)

The rest of the application is just to display the current status of the MQTT server and also the incoming JSON string sent by the button.

![image](/assets/2025-02-21/070.png)

## result and conclusion

The screenshot shows the result after the button has been pressed. As we can see in the JSON string the type of is "S" which means the button has been pressed only once for a short moment.

![image](/assets/2025-02-21/080.png)

