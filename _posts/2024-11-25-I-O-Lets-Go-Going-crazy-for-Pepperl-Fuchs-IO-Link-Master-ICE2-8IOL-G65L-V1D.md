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
In this blog, we already discussed the usage of various ways to integrate sensors into a Peakboard application. A summary of these articles is available in [The hitchhiker's guide to I/O devices](/I-O-Lets-go-The-hitchikers-guide-to-I-O-devices.html). In today's article we will have a look at a very sophistcated and a bit more expensive type of I/O modules - an IO-Link master. The big main difference to the other I/O modules is, that an IO-Link doesn't use just blank wires to connect the sensors, but instead implements the IO-Link plug. So the sensor must offer this IO-Link plug to be used with an IO-Link master.

The IO-Link Master we use in this article will be the Pepperl+Fuchs IO-Link-Master ICE2-8IOL-G65L-V1D.

![image](/assets/2024-11-25/010.png)

And the sensor we will use is a Pepperl+Fuchs UC250-F77-EP-IO-V31. This is an ultrasonic distance sensor that meassures the distance to the next object located in the range of the sensors with the help of ultrasonic sound waves. It can be used to check, if an area is occupied by something (e.g. if there is a person standing nearby) or an object is passing by, and if so, what is the distance between the sensor and the object.

![image](/assets/2024-11-25/020.png)

## IO-Link Master Configuration

Like other I/O modules the IO Link Master comes with a web interface. The idea of IO-Link principle is that the way an IO Link device works is described in a so called IODD file (IO Device Description). The easiest way to get the IODD file for our sensor is to go to the website [https://ioddfinder.io-link.com/] and look up the exact sensor name and then download the IODD from there.

In the web interface of our IO Link master, we can find the section "Attched Devices -> IOD files" and upload the file there to the master.

![image](/assets/2024-11-25/030.png)

When we switch to the Port 3 tab, because we pluuged the ensor into the physcial port 3, we can see the details of the sensor provided by the vendor in the IODD file. The matching between the physical sensor and the IODD file happens with the help of the unique product id.

![image](/assets/2024-11-25/040.png)

## Accessing the data from Peakboard

There are several ways to access the IO Link master from the data consumer side. Of course it comes with a built-in MQTT connection, which is always a nice choice. But for the next step, we will make use of the built-in OPC UA server endpoint that is also provided. Using OPC UA over MQTT in that context has two advantages to consider:

- The OPC UA option comes with a server, so we can connect Peakboard directly to the IO Link master, while MQTT always needs a broker in between.
- As through IODD file, the IO Link master knows exactly what kind of data is coming from he sensor and how it must be interpreted. So we don't need to worry too much about looping though JSON or other data conversions. We can just drag & drop the ready-to-use value from the OPC UA source.

After we created a new data source to OPC UA, we add two new subscription. The OPC UA nodes can be just drilled through and here we see the full power of this IO Link procedere with IODD file: We can see, that already the correct metadata is shown under the corresponding port and attached device. The OPC UA server already "knows" that this sensor has two relevant data values, the distance and the state if there's an object within the scope of the sensor. And also the correct data types are reflected there. We can even see a brief explanation of the value on the right side.

![image](/assets/2024-11-25/050.png)

So that's how the fully set up data source looks like.

![image](/assets/2024-11-25/060.png)

The board itself is easy to build. We just bind the OPC UA source to a text field and to an icon that switches its appearance according to a conditional formatting rule.

![image](/assets/2024-11-25/070.png)

## result

The following gifs are showing the final result. The object in the cone of the sensor causes the corresponding value to be flagged as occupied and also gives the corect disctance between the sensor and the object.

![image](/assets/2024-11-25/result0.gif)

![image](/assets/2024-11-25/result1.gif)





