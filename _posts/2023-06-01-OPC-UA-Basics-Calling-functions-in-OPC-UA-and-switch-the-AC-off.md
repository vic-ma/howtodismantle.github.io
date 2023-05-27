---
layout: post
title: OPC UA Basics Calling functions in OPC UA and switch the AC off
date: 2023-03-01 12:00:00 +0200
tags: opcua tutorial
image: /assets/2023-06-01/title.png
---
 In this [article]({% post_url 2023-05-15-OPC-UA-Basics-Getting-started-with-a-public-OPC-UA-server %}) we learned how to subscribe to OPC-UA-nodes and process the incoming values. The sample was around an A/C that provided several values like the temperature. Make sure to read this article first because today we will learn how to switch the A/C on and off by using so called OPC-UA-functions.

 At this point I strongly recommand downloading another very useful tool called [UA Expert](https://www.unified-automation.com/products/development-tools/uaexpert.html). It's free of charge and the perfect tool to play around with any kind of OPC UA server. We will also use it in this tutorial.
 After opening the UA Expert tool, right click on the servers node on the upper left pane and add the server opc.tcp://opcuaserver.com:48010, that we already know. AFter connection is established succesfully, you find the same object structure in the pane on the left side that we already know from subscribing to various nodes in Peakboard. If you want to play around with subscriptions, just drag and grop a node to the middle pane.

![image](/assets/2023-06-01/010.png)

Beside the regular nodes there are more ones with a pink box icon. These represent functions. In our sample case, there are functions for starting and stopping the A/C machine. To try out, right click on the function node and click on _Call_. Whe you have scubscribed to he temperature or state node before, you can see, that the temperature value starts to contantly go up, while state is switched to 0 (= off), because we obviously switched the A/C off.

![image](/assets/2023-06-01/020.png)

### How to use OPC UA function in Peakboard?

Before we start we need some more technical things: The node ID of the function we want to call, and the node ID of the object that this function belongs to. You will find the node IDs in the attribute's pane on the upper right corner in UA Expert.

![image](/assets/2023-06-01/030.png)

Let's go back to our sample board and add a button to the canvas. Unfortunately there's no Building Block available for calling an OPC UA function, so we need to add a line of scripting. However that's not too complicated. In the right function helper pane just browse to Functions -> Publisch to external systems -> MyOPCDataSource -> Call Function and drag and drop the line to he editor.
This script function needs two parameters: The node ID of the object where the function belongs to and the node ID of the function itself.

![image](/assets/2023-06-01/040.png)

Here's the original script line in case you want to copy and paste. Please note, that the id of the shared connection must be adjusted before using it.

{% highlight lua %}
connections.getfromid('FaaQ/H+xDfhKpywNrZl64jMXlx0=').callmethod('ns=3;s=AirConditioner_1', 'ns=3;s=AirConditioner_1.Start')
{% endhighlight %}

To make our sample more beautiful, we add two buttons for each A/C. One switching it ON and one for switching it OFF. And also we put an icon on the canvas to indicate the _State_ of the A/C with he help of some conditional formatting (see other tutorial or just download the sample).

![image](/assets/2023-06-01/050.png)

And here's how it looks like in action. As soon as we switch the A/C off, the ON/OFF icon turns dark (conditional formatting on the subscribed _State_ node) and the temperture starts to rise. When swithcing it _On_ again, the cooling action start and the temperature is going down again...

![image](/assets/2023-06-01/060.gif)



