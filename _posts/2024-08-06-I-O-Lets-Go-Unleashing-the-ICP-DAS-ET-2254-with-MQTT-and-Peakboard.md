---
layout: post
title: I/O, Let's Go - Unleashing the ICP DAS ET-2254 with MQTT and Peakboard
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt
image: /assets/2024-08-06/title.png
read_more_links:
  - name: ICP DAS ET-2254 website
    url: https://www.icpdas.com/en/product/ET-2254
downloads:
  - name: ET2254TestBoard.pbmx
    url: /assets/2024-08-06/ET2254TestBoard.pbmx
---
[ICP DAS](https://www.icpdas.com/) is a Taiwan-based company that sells industrial components for automation. In today's article, we will take a closer look at [ET-2254](https://www.icpdas.com/en/product/ET-2254), a 16 channel I/O module that provides digital input and output pins, and comes with a cool MQTT interface that makes it easy to create a stable connection to a Peakboard app.

Most customers use the ET-2254 to connect simple sensors to Peakboard, like physical buttons or light barriers. On the output side, we often see customers using it to operate traffic lights and do other binary stuff.

Here's what the ET-2254 looks like:

![image](/assets/2024-08-06/010.png)

And here's what the setup looks like for the example we're building in this article. A button is mounted to one of the pins that is later used as an input channel.

![image](/assets/2024-08-06/015.jpg)

## The configuration

The ET-2254 comes with a web interface that offers a huge variety of functions to configure the device in its behaviour and connections. In our case we only look at the main options to configure MQTT, get the input signal for a physical button and give an output singal to switch on a traffic light.

The 16 pins can be used for both Input and Output. The first thing we do under the tab "I/O settings" is to define which pins are treated as input and which are treated as output. In our example we just do half half and define the pins D0 to D7 as output and the rest as input.

![image](/assets/2024-08-06/020.png)

Under the tab "MQTT" we connect the device to an MQTT broker. In our case we just choose test.mosquitte.org and use the main topic "dismantle". All MQTT message exachangs happen under this main topic.

![image](/assets/2024-08-06/030.png)

Let's jump to the input setting under the "DI" tab. To make sure, that every state change generates an MQTT message, we must change the setting "State-Change-Publish" to "Yes".

![image](/assets/2024-08-06/040.png)

And the same we do in the other direction. The "DO" tab makes it possible for the device to listen to certain topics and then change the state of the output according to the message that is received under this topic.

![image](/assets/2024-08-06/050.png)

## Building the Peaboard application for the Input

The MQTT configuration is quite simple. We just subscribe to the topic for the digitial input "DI08".

![image](/assets/2024-08-06/060.png)

In the "Refreshed" script we check the incoming message. If it's 1, the state of the input has changed from "no signal" to "signal", a 0 indicates the opposite. So every time a "1" is submitted we count our counter variable "CounterDI08" up by 1.

![image](/assets/2024-08-06/070.png)

The next thing we do is put an icon on the canvas. In out case a gray symbol. With using some Conditional Formatting, we can define to make the icon green in case the MQTT message is "1" which symbolyzes that the button is pressed or the Input signal is set to "on". Otherwise it goes back to the default state of gray.

![image](/assets/2024-08-06/080.png)

Here's the result when the physical button is pressed generating the MQTT message. We note in the video sequence that our Peakboard application (on the right) reacts much faster than the web interface (on the left) which lags behind some fractions of a second.

![image](/assets/2024-08-06/result1.gif)

## Building the Peaboard application for the Output

Setting the output signal on the channel DO08 is done just through sending a "1" or "0" message to the MQTT topic we configured earlier. We'reusing a toggle control to switch the channel n and off. 
The screenshot shows how to send an MQTT message through the MQTT Bulding Block in the "Toggle" event of the control. For all the LUA lovers the LUA code can be also seen at the bottom.

![image](/assets/2024-08-06/090.png)

And here's the result shown in a short video sequence. On the right is our toggle button. On the left we can see the state of the output as visualized in the web interface of the device. We still note a lag between sending the message and the actual reaction in the web interface. This is a pure web interface problem. The actual output channel is switched according to the MQTT message without any long latency, only the web interface needs a couple of milli seconds.

![image](/assets/2024-08-06/result2.gif)