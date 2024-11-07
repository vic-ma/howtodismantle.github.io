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
The German company Bosch offers an innovative camera that called FLEXIDOME indoor 5100i IR - 8MP. Not only is it a regular security camera, but it also comes with a couple of interesting AI features for monitoring the public space. 

The camera can detect objects that are in its view. For example, it can detect people or vehicles and do a lot of magical stuff with this information. 

In this article, we will give a brief overview of how to configure the Bosch cam and connect it to a Peakboard application. In the [second part of this Bosch cam series](/Cam-like-a-Bosch-Part-II-Inegrate-Cam-Images-and-Streams-into-Peakboard-Applications.html), we will discuss the options for integrating the camera image into a Peakboard dashboard.

There's also a nice [Youtube video](https://www.youtube.com/watch?v=ztthsCF4USw&ab_channel=PeakboardEN) that covers this topic.

The following picture highlights the problem we want to solve. It shows a bird's-eye view of a parking lot. As you can see, there are two rows of parking.

We call the upper row of parking spots Lot A, and we call the bottom row Lot B. The objective is to build a dashboard that shows the current level of occupancy for these two lots.

![image](/assets/2024-11-09/010.png)

## Configuring the cam

Every Bosch cam comes with a web interface. However, the more advanced settings must be changed through a desktop application called the "Configuration Manager."

The first thing we do is add the cam to the Configuration Manager. After, we connect the cam to an MQTT broker. In our case, we choose the Peakboard MQTT broker for the templates, but any broker will do.

![image](/assets/2024-11-09/020.png)

The real magic happens in the **VCA** tab. Here, we can add "tasks." A task represents an action that the cam performs when something happens in the cam's view. For our project, we need two tasks, one for Lot A and one for Lot B.

We can add or change the cam's tasks in the Configuration Manager.

![image](/assets/2024-11-09/030.png)

We need to select a trigger for our tasks. In our case, the trigger is "Object in field."

![image](/assets/2024-11-09/040.png)

We then link the task to an actual field where the task is applied to. The following screenshot shows the task for Lot A. You can see that the field is highlighted on the right side and integrated into the cam picture.

![image](/assets/2024-11-09/050.png)

We also set the intersection trigger to "edge of box." That way, even if only part of a car is in the field, it still triggers the event. Of course, we could also make it so that the whole car must be in the field, but that doesn't make sense in our case.

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





