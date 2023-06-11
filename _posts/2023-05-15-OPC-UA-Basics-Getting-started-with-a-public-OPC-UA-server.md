---
layout: post
title: OPC UA Basics - Getting started with a public OPC UA server
date: 2023-05-15 12:00:00 +0200
tags: opcuamqtt
image: /assets/2023-05-15/070.gif
---
OPC UA might be a complicated topic for some people and if you're new to UPC UA this is exactly the right starting point. Minimum Requirements except having the Peakboard Designer installed for this tutorial: Zero! You can start right away, because we're using an OPC UA server that is available to the public.
Check out the website [opcuaserver.com](http://opcuaserver.com/) to find out more about publicly available servers. The one we're using is opc.tcp://opcuaserver.com:48010

FInd the downloadable pbmx [here](/assets/2023-05-15/AirConditionMointoring.pbmx)

## Setting up server connection

In the Peakboard Designer create a new OPC UA data source.

![image](/assets/2023-05-15/010.png)

Enter a name, and the above mentioned URL opc.tcp://opcuaserver.com:48010 and click on the recycle button to get the end points. Why are there several end points? Because the server offers different types of security. You can use it with or without authentification and different levels of encryption. We use the easiest: just "None" and also leave the Authentifiction setting on "Anonymous".

![image](/assets/2023-05-15/020.png)

Before we can really connect to the server, we need a certificate to identify ourself. If you don't have one, no problem, we can easily create one. Just click on Load client certificates, Create and fill out the basic information and click on create, the select. That's it.

![image](/assets/2023-05-15/030.png)

We can now hit the Connect button to check the connection and then click on Manage Subcriptions. If everything works well you can see a hierarchy of so called nodes. Every OPC UA server offer these nodes that represent hierarhcically organised data points. Feel free to browse the hierarchy. This demo server offers all kinds of demo data with different data types to experiment. For our sample we browse the hierarchy and find a couple of "Air Conditioners". These represent real A/Cs with certain attributes. The State says, if the A/C is switched on or off. Temperature is (obviously) the actual temperature while TemperatureSetPoint represents the temperture where it should be. The last attribute we're interested in is PowerConsumption. If this attribute is set to greater than zero, the A/C is doing actual cooling. After selecting these attributes return to the main dialog.

![image](/assets/2023-05-15/040.png)

Now you already can try out to get some real values from the server. Just click on Enable Listener and see how the real numbers are flowing in. The actauly temperature is changing every couple of seconds. And as soon as the temperature rises above the set point, the power consumption goes up because the A/C will start to cool down again.

![image](/assets/2023-05-15/050.gif)

Now we can start to really use the data. The details you can find out yourself just by downloading the pbmx. It use some tiles to present the data. The pbmx show a second A/C (same pattern, just different nodes) and also show an icon control that changes its color with the help of conditional formatting (no scripting).
Feel free to check out this article to find out more about Conditional Formatting.
Every time the power consumption goes up (actual cooling), the snowflake turns into a bright color.

![image](/assets/2023-05-15/060.png)

And here's how it looks like in real life.

![image](/assets/2023-05-15/070.gif)
