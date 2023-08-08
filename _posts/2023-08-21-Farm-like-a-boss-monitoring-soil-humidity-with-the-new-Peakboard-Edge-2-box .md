---
layout: post
title: Farm like a boss - Monitoring soil humidity with the new Peakboard Edge 2 box
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2023-08-21/title.png
read_more_links:
  - name: The sensor used in this article at Amazon (Germany)
    url: https://www.amazon.de/Feuchtesensor-0-10V-2-Bodentemperatur-Bodenfeuchtesensor-Sender/dp/B082DJ6F9J/ref=sr_1_30?__mk_de_DE=%C3%85M%C3%85%C5%BD%C3%95%C3%91&crid=B8RQX4B0GNT7&keywords=feuchtesensor+24v&qid=1683540626&sprefix=feuchtesensor+24v%2Caps%2C92&sr=8-30
downloads:
  - name: EdgePlantMonitoring.pbmx
    url: /assets/2023-08-21/EdgePlantMonitoring.pbmx
---

In fall 2023, a new version of the Peakboard Edge box was released. Version 1 was released in 2019 and only allows for very simple sensors with an on/off state (e.g. light sensors). Edge 2 allows for and fully supports analogue sensors that operate in a certain range.

This article is a step-by-step guide on how to setup an Edge 2 box and use a humidity and temperature sensor to display the humidity and temperature in a Peakboard application. 

The sensor we use in this article can be found on [Amazon](https://www.amazon.de/Feuchtesensor-0-10V-2-Bodentemperatur-Bodenfeuchtesensor-Sender/dp/B082DJ6F9J/ref=sr_1_30?__mk_de_DE=%C3%85M%C3%85%C5%BD%C3%95%C3%91&crid=B8RQX4B0GNT7&keywords=feuchtesensor+24v&qid=1683540626&sprefix=feuchtesensor+24v%2Caps%2C92&sr=8-30). But any sensor of the same kind will do. They all work in the same way and follow the same pattern.

Here's how the sensors looks like after unpacking:

![image](/assets/2023-08-21/010.jpg)

## Connect sensor to Edge 2

Edge 2 offers four I/O ports. Each of them comes with three plugs:

* Power supply positive
* Power supply negative
* Sensor output current

This picture shows how the brown wire (positive) and the black wire (negative) must be connected to the first I/O port.

As the sensor has two outputs (temperature and humidity), we mount the first to the current input of I/O 1 and the second to the current input of I/O 2. The second sensor (and I/O 2) doesn't need additional power. The power for both sensors comes from the brown and black cables.

Please note that the documentation here is wrong. It says that a gray cable represents the temperature. In fact, the cable is green.
  
![image](/assets/2023-08-21/020.jpg)

After that, we're pretty much done and can now boot up the box and connect it to a network.

## Set up Edge 2 in Designer

### Adding the Edge box

With the Peakboard Designer, there's a special dialog for maintaining all Edge boxes. Just add the box with its IP address and credentials to the box collection. You might want to use the *Connections* tab to connect it to Wi-Fi.

![image](/assets/2023-08-21/030.png)

Here's the most important part: configuring the ports. We can see in the sensor documentation that the temperature sensor covers a range of -45 to 115 degrees Celsius, which is represented by a current of 0-10 V. This is what we put in the translation table.

![image](/assets/2023-08-21/040.png)

We do the same thing for humidity. Here, the documentation says that the maximum humidity is represented by 10 V. However, when we actually put the whole sensor into a glass of water, the value goes up to 4.92 V. That's why we let 5 V represent a humidity of 100%. Maybe the Chinese manufacturer of the sensor calculates humidity in a different way. Who knows?

![image](/assets/2023-08-21/050.png)

### Creating the dashboard

Finally, we need to add an Edge data source.

The previous step was only to set up the Edge box itself. And we only need to do that once. Every time we actually use the Edge box, we can just add a data source for the box. That's it. All the configuration is already done.

![image](/assets/2023-08-21/055.png)

The Peakboard canvas itself consists of a couple of icon controls and text boxes that are bound to the Edge data source. The only thing to mention here is that you should carefully check the unit and format of the values:

![image](/assets/2023-08-21/060.png)

And then we're set. Here's what the board looks like in real life:

![image](/assets/2023-08-21/070.jpg)
