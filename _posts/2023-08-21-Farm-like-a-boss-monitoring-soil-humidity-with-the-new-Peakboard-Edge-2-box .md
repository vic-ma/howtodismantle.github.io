---
layout: post
title: Farm like a boss - monitoring soil humidity with the new Peakboard Edge 2 box
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
As by fall 2023 there's a new version of the Peakboard Edge Box released. Version 1 dated back to 2019 and only allows very simple sensors with a on/off option (e.g. light sensors). Edge 2 allows fully supports analoque sensors that operate in a certain range. This article covers a step-by-step guide on how to setup a Edge 2 box and use a humidity and temperature sensor to display these two values in a Peakboard application. 

The sensor we use in this sample can be found at [Amazon](https://www.amazon.de/Feuchtesensor-0-10V-2-Bodentemperatur-Bodenfeuchtesensor-Sender/dp/B082DJ6F9J/ref=sr_1_30?__mk_de_DE=%C3%85M%C3%85%C5%BD%C3%95%C3%91&crid=B8RQX4B0GNT7&keywords=feuchtesensor+24v&qid=1683540626&sprefix=feuchtesensor+24v%2Caps%2C92&sr=8-30). But any sensor of the same kind will do it. They all work in the same way and follow the same pattern.

Here's how the sensors looks like after unpacking:

![image](/assets/2023-08-21/010.jpg)

## Connect sensor to Edge 2

Edge 2 offers four I/O ports. Each of them comes with three plugs: Power Supply Positive / Negative and the actual sensor output current. The picture shows how the brown (positive) and the black (negative) must be connected to the first I/O port. As the sensor has two outputs (temperature and humidtiy), we mount the first to the current input of I/O 1 and the second to I/O 2. The second sensors (and I/O 2) doesn't need additional power. The power for both sensors comes from the brown and black cables. Please note, that the documentation is wrong. It says, that a gray cable is represents the temperature. In fact the cable is green.
  
![image](/assets/2023-08-21/020.jpg)

After that we're pretty much done and now can boot the box and connect it to a network.

## Setup Edge 2 in designer

With the Peakboard Designer there's a special dialog to maintain all Edge boxes. Just add the box with its IP address and credentials to the box collection. You might want to use the 'Connections' tab to connect it to WiFi.

![image](/assets/2023-08-21/030.png)

The most important part comes with configuring the ports. We can read in the sensor documentation, that the temperature sensor covers a range of -45 to 115 degress Celcius, which is represented by a current of 0-10V. This is what we put in the translation table.

![image](/assets/2023-08-21/040.png)

We do the same for humidity. Here the documentation says, that the maximun humidity is represented by 10V. However when we put the whole sensor into a glass of water, the value goes up to 4.92V. That's why we define, that 5V actually repesents a humidity of 100%. Maybe the Chinese manfacturer of the sensor calculcates humidity in a different way. Who knows?

![image](/assets/2023-08-21/050.png)

Finally we need to add a Edge datasouce to out projects. The previous step is only to set up the box itself and once. Every time we actually use it, we only need a data source where we add the box. That's it. All the configuraton steps are already done...

![image](/assets/2023-08-21/055.png)

The Peakboard canavas itself consists of a couple of icon controls and text boxes that are bound to Edge datasource. the only thing to mention is to carefully check for the unit and format of the values:

![image](/assets/2023-08-21/060.png)

And then we're fully set. Here's how the board looks like in real life:

![image](/assets/2023-08-21/070.jpg)
