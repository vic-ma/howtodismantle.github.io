---
layout: post
title: Cam like a Bosch - Part I - Integrate Bosch security cam AI features with Peakboard
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2024-11-09/title.png
read_more_links:
  - name: Peakboard Hardware Guide
    url: /hardwareguide
  - name: Bosch Cam Product Page
    url: https://www.boschsecurity.com/us/en/news/product-news/flexidome-5100i/
  - name: YouTube - Integrate a Bosch IP camera system on your dashboard
    url: https://www.youtube.com/watch?v=ztthsCF4USw&ab_channel=PeakboardEN
  - name: Cam like a Bosch - Part II - Integrate Cam Images and Streams into Peakboard Applications
    url: /Cam-like-a-Bosch-Part-II-Integrate-Cam-Images-and-Streams-into-Peakboard-Applications.html
downloads:
  - name: BoschCamParkingLotMonitoring.pbmx
    url: /assets/2024-11-09/BoschCamParkingLotMonitoring.pbmx
---
The German company Bosch offers a very innovative camera that calls itself FLEXIDOME indoor 5100i IR - 8MP. Ad it's not only a regular security camera but it comes with a couple of nice and interestig AI features for monitoring the public space. The camera can detect objects that are in its sight, e.g. people or different kinds of vehicles and do a lot of magical stuff with this information. 

In this article we will discuss a brief overview on how to configure the Bosch cam and connect it to a Peakboard application. In a [second part of this Bosch cam series](/Cam-like-a-Bosch-Part-II-Inegrate-Cam-Images-and-Streams-into-Peakboard-Applications.html), we will discuss the options to integrate the camera image into the dashboard.

Please note, that there's also a nice [Youtube video](https://www.youtube.com/watch?v=ztthsCF4USw&ab_channel=PeakboardEN) available discussing this topic.

The problem we want to solve can be seen in the next picture. It shows the bird view of a parking lot. As we can see, there are two rows to park multiple cars. The upper stripe of parking spots we call Lot A and the bottom one we call Lot B. The objective is to build a dashboard that shows the current level of oocupancy for these two stripes.

![image](/assets/2024-11-09/010.png)

## Configuring the cam

Every Bosch cam comes with a web interface. However the more sophisticated settings must be done through a desktop application called the "Configuration Manager". The first thing we must do after adding the cam to the configurartion manager is to connect the cam to an MQTT broker. Here in our case we just choose the Peakboard MQTT broker for the templates, but any broker will do it.

![image](/assets/2024-11-09/020.png)

The actual miracle is happening in the VCA tab. Here we can add so called tasks. A task represents a certain action how the cam is supposed to react when something happens within the cam's sight. For our project we need two tasks, one for Lot A and one for Lot B.

We can add or change the tasks in the configuration manager.

![image](/assets/2024-11-09/030.png)

When creating a task the cam comes with different actions. In our case the action is "Occupancy".

![image](/assets/2024-11-09/040.png)

In the occupancy task we have to link the task to an actual field where the task is applied to. The screenshot shows the task for Lot A. we can see the field is highlighted on the right side and integrated in the cam picture. Furthermore we need to define the so called Interscetion trigger. It defines, that even a small edge of the destination object in the field triggers the event. Of course we could also define, that the whole car must be located within the field, but that doesn't make sense in our use case.

![image](/assets/2024-11-09/050.png)

![image](/assets/2024-11-09/060.png)

## MQTT and configuring the data source

Let's check how our task configuration looks like on the MQTT side.
The cam generates a MQTT message every time a task is triggered or in whatever form changes. The screenshot shows the raw view on the MQTT message and the tree of different topics down to the actual payload.

![image](/assets/2024-11-09/070.png)

Let's have a look at the Peakboard side. In the data source we defined two sbscriptions with the complex topics. By using the simple data path "Data.Count" it's not necessary to implement a script to get the actual value from the JSON message. The Peakboard engine is doing it for us and translates the JSON to a useful payload that can be used right away.

![image](/assets/2024-11-09/080.png)

The rest is almost not worth to mention. We just bind a nice tile to the two values of the data source.

![image](/assets/2024-11-09/090.png)

## Result

If we take a final look at the result, we can see, how good the occupancy detection works. As soon as car enters or leaves the two fields, the data is adjusted and is reflected in the Peakboard board. Cars outside of the fields are not counted.

![image](/assets/2024-11-09/100.png)





