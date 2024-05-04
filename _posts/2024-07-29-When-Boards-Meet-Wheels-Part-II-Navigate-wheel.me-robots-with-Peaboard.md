---
layout: post
title: When Boards Meet Wheels - Part II - Navigate wheel.me robots with Peaboard 
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2024-07-29/title.png
read_more_links:
  - name: When Boards Meet Wheels - Part I - How to connect Peakboard and wheel.me robots
    url: /When-Boards-Meet-Wheels-Part-I-How-to-connect-Peakboard-and-wheel.me-robots.html
  - name: wheel.me website
    url: https://www.wheel.me/
  - name: Peakboard wheel.me Extension
    url: https://templates.peakboard.com/extensions/Wheel-Me/en
downloads:
  - name: WheelMePlayground.pbmx
    url: /assets/2024-07-29/WheelMePlayground.pbmx
---
In the [first part](/When-Boards-Meet-Wheels-Part-I-How-to-connect-Peakboard-and-wheel.me-robots.html) of our series on how to build Peakboard applications and integrate it with wheel.me, we learned how to connect to the wheel.me API and get information about floors, positions, and robots.

In this article, we will learn how to move the robots, and how to build a command stand for the wheel.me fleet inside a production environment.
Just as a reminder, we will stick to the floor and positions of the simulation environment, as shown in the map from the wheel.me portal:

![image](/assets/2024-07-29/010.png)

## Move the robot

You can send a command to move the robot by using a function that is provided by the wheel.me extension. To be more precise, the function is a part of the robot's data source. To trigger the function with Building Blocks, we use the **Run Function** block, and select the robots list. All available functions are shown.

To command the robot to a certain position there are two function available depending on if we want to use the name of the position or the ID. In our case the name will do it, so we use "NavigateToPositionName".

* The robot ID is taken from the first line of the robots data source
* The floor ID is taken from a variable to make things a bit more dynamic
* The destination position is just the name. In our sample WH1 is the starting point. So we use this function to order the robot back to the starting point.

![image](/assets/2024-07-29/020.png)

For all the script lovers out there, the function is also available in LUA:

{% highlight lua %}
data.MyRobots.NavigateToPositionName(data.MyRobots.first.ID, data.MyFloorID, 'WH1')
{% endhighlight %}

## Building an application

In this part we can have a look at a simple application. Let's say we start the robot's mission always at position WH1 and want him to go to in circles to the 4 more points WP01-WP04. As the screenshot shows, the user can enable or disable certain position. If a position is disabled the robot does not stop there but goes on to the next available position. When the worker at a certain workplace is finished, we click the "Next Goal" button to command the robot to the next position.
An icon next to the position indicates where the robot is currently located. On the right side the current meta data of the robot is shown (like the coordinates, the next position and the current state).

![image](/assets/2024-07-29/030.png)

The robot information on the right side is just directly bound to the robots data source, to show how the robot is moving and at which position he's currently located.

![image](/assets/2024-07-29/040.png)

Let's have a look what is behind the "Start Mission" button. We can see at the Building Blocks that we check any of the workplace related toggle buttons. The next available is used to send the command to the wheel.me API and store the destination in a variable.

![image](/assets/2024-07-29/050.png)

Every other workplace works similiar but only checks the remaining toggle buton / workplace.

![image](/assets/2024-07-29/060.png)

On last screenshot shows the icon. There's an icon at every position and we just put it to "visible = true" in case the icon's position is the same as the robots position.

![image](/assets/2024-07-29/070.png)

## result

In the animated gif we can see how the app works. In the left part is the Peakboard app, in the right part there's the wheel.me web portal. The mission is started by clicking the button. The command is sent to wheel.me API and the robot is sent to the new position WP01. After the robot has left the WH1 position the icon dissapears and the X/Y coordinates are constantly changing. As soon as the robot is about to arrive at WP01 the icon also pops up at the WP01 place in the app.

![image](/assets/2024-07-29/result.gif)

