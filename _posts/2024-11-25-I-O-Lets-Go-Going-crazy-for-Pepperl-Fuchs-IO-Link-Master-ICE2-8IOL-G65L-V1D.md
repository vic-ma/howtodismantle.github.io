---
layout: post
title: I/O, Let's Go - Going crazy for Pepperl+Fuchs IO-Link-Master ICE2-8IOL-G65L-V1D
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt api
image: /assets/2024-11-25/title.png
image_header: /assets/2024-11-25/title_landscape.png
read_more_links:
  - name: I/O, Let's go - The hitchhiker's guide to I/O devices
    url: /I-O-Lets-go-The-hitchikers-guide-to-I-O-devices.html
  - name: Pepperl+Fuchs Website
    url: https://www.pepperl-fuchs.com/germany/de/classid_4996.htm?view=productdetails&prodid=96749
downloads:
  - name: PFIOLinkMaster.pbmx
    url: /assets/2024-11-25/PFIOLinkMaster.pbmx
---
In this blog, we've discussed different ways to integrate sensors into a Peakboard application. You can see an overview of these articles in [The hitchhiker's guide to I/O devices](/I-O-Lets-go-The-hitchikers-guide-to-I-O-devices.html).

In today's article we'll take a look at a sophisticated and slightly more expensive type of I/O module---an IO-Link master.

The main difference between the IO-Link master and other I/O modules is that the IO-Link doesn't use blank wires to connect the sensors. Instead, it uses the IO-Link plug. A sensor must provide this IO-Link plug in order to be used with an IO-Link master.

The IO-Link master we'll use in this article is the Pepperl+Fuchs IO-Link-Master ICE2-8IOL-G65L-V1D.

![image](/assets/2024-11-25/010.png)

And the sensor we'll use is the Pepperl+Fuchs UC250-F77-EP-IO-V31. This is an ultrasonic distance sensor that measures the distance to the next object that's within range of the sensors, with the help of ultrasonic sound waves. 

It can check if an area is occupied by something (e.g. if there is a person standing nearby). It can also tell if an object is passing by and how far away it is.

![image](/assets/2024-11-25/020.png)

## IO-Link master configuration

Like other I/O modules, the IO-Link Master comes with a web interface. The idea behind the IO-Link is that the way an IO-Link device works is described in an IODD file (IO device description file).

The easiest way to get the IODD file for our sensor is to go to the [IODD Finder website](https://ioddfinder.io-link.com/) and enter our sensor's exact name.

Then, we go to the web interface for our IO-Link master and navigate to **Attached Devices > IODD FILES**. There, we can upload the IODD file to the IO-Link master.

![image](/assets/2024-11-25/030.png)

We plugged our sensor into the physical port 3. So, when we switch to the **Port 3** tab, we can see the details of the sensor provided by the vendor in the IODD file. The match between the physical sensor and the IODD file happens with the help of the unique product ID.

![image](/assets/2024-11-25/040.png)

## Accessing the data from Peakboard

There are several ways to access the IO-Link master from the data consumer side. Of course it comes with a built-in MQTT connection, which is always a nice choice. But for the next step, we will make use of the built-in OPC UA server endpoint that is also provided. Using OPC UA over MQTT in that context has two advantages to consider:

- The OPC UA option comes with a server, so we can connect Peakboard directly to the IO-Link master, while MQTT always needs a broker in between.
- As through IODD file, the IO-Link master knows exactly what kind of data is coming from he sensor and how it must be interpreted. So we don't need to worry too much about looping though JSON or other data conversions. We can just drag & drop the ready-to-use value from the OPC UA source.

After we created a new data source to OPC UA, we add two new subscription. The OPC UA nodes can be just drilled through and here we see the full power of this IO-Link procedere with IODD file: We can see, that already the correct metadata is shown under the corresponding port and attached device. The OPC UA server already "knows" that this sensor has two relevant data values, the distance and the state if there's an object within the scope of the sensor. And also the correct data types are reflected there. We can even see a brief explanation of the value on the right side.

![image](/assets/2024-11-25/050.png)

So that's how the fully set up data source looks like.

![image](/assets/2024-11-25/060.png)

The board itself is easy to build. We just bind the OPC UA source to a text field and to an icon that switches its appearance according to a conditional formatting rule.

![image](/assets/2024-11-25/070.png)

## result

The following gifs are showing the final result. The object in the cone of the sensor causes the corresponding value to be flagged as occupied and also gives the corect disctance between the sensor and the object.

![image](/assets/2024-11-25/result0.gif)

![image](/assets/2024-11-25/result1.gif)





