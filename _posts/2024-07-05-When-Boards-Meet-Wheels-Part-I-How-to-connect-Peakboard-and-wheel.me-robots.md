---
layout: post
title: When Boards Meet Wheels - Part I - How to connect Peakboard and wheel.me robots
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2024-07-05/title.png
read_more_links:
  - name: When Boards Meet Wheels - Part II - Navigate wheel.me robots with Peaboard 
    url: /When-Boards-Meet-Wheels-Part-II-Navigate-wheel.me-robots-with-Peaboard.html
  - name: wheel.me website
    url: https://www.wheel.me/
  - name: Peakboard wheel.me Extension
    url: https://templates.peakboard.com/extensions/Wheel-Me/en
---
[Wheel.me](https://www.wheel.me/) is a Norwegian company with a revolutionary concept for building autonomous vehicles. They don't actually sell an entire vehicle. Instead, they sell four intelligent wheels that can be mounted to all kinds of pallets or other trays, to transport them within a factory. The wheels have access to the local WiFi and connect to a software backbone that can be either hosted in AWS or on prem.
This article shows how to use Peakboard to build an interface to interact with the wheel.me backbone. First we will have a look on how to get information about the environment like the floors, position and robots. In [the second part of this topic](/When-Boards-Meet-Wheels-Part-II-Navigate-wheel.me-robots-with-Peaboard.html) we focus and the actual commanding robots to certain position and build an intractive logic for the wheel.me environment.

## Setting up the the extension

The wheel.me API is actually not too complicated to be used in Peakboard natively. But to make it even much easier, we can use the wheel.me extension that is available in the extension menu. For informations about how to install an extension, see the [Manage extensions documentation](https://help.peakboard.com/data_sources/Extension/en-ManageExtension.html).

![image](/assets/2024-07-05/010.png)

After the installation there are three lists availbale. For the floors, the positions on a certain floor and the robots on certain floor.

![image](/assets/2024-07-05/020.png)

You will need a regular account to access the API of he wheel.me backbone with enough rights to do so. All three lists of the extension will need the credentials (user name and password) along with the Base URL, usually in the form of "https://XXX.wheelme-web.com/". We make sure to end the base URL with a "/".

The screenshot shows the "floors" list. After entering the basic data we can list all available floors. It's important to remember the floor id, as we will need it later when it comes to the robots.

![image](/assets/2024-07-05/030.png)

## Query the positions

In our sample environment we have a total of 6 positions. Beside the 5 regular points there's also one position for charging. Our floor has only one robot and as we see in the map, it is located in position WH1 while facing to the north.

![image](/assets/2024-07-05/040.png)

Let's check the output of the "position" data source. We must provide to floor ID to identify the floor we want query. In the list we can see all positions from the above screenshot along with the metadata like the physical coordinates and the current state. States can be "Free" or "Reserved". And we can also see, that the position WH1 is occupied with a robot. 

![image](/assets/2024-07-05/050.png)

## Query the robots

The robot data source is the most important to show the dynamic aspects of the scenery. As shown in the screenshot we need to provide the floor ID again. We can also provide a name filter for the name. This might be important when we have multiple robots on the floor. Then it would be a option to make sure, that there's always one dedicated robot per data source by using this filter. Then it's much esier to access the data of robot because we don't need to look up the robot in a list but always take the first line of the data source. But again, this only applies when we have more than one robot on the floor.
Beside the physical coordinates we can see the state (like "Navigating" or "Idle") and the mode (like "Autonmous" or "manual"). Most important is the location information. The position, the robot is navigating to and also the position the robot is located at (for each both the ID and the name of the position is provided).

![image](/assets/2024-07-05/060.png)

## Result and conclusion

Obviously getting the necessary metadata along with the robot position data is not too complicated. With these sources it should be no problem to build Peakboard applications that show the location and status of wheel.me robots.
In the [second part part of this article](/When-Boards-Meet-Wheels-Part-II-Navigate-wheel.me-robots-with-Peaboard.html) we will learn how to command the robot to certain positions and build a logic around missions.
