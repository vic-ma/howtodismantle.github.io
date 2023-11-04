---
layout: post
title: Bed time stories - Three ways to shut down a Peakboard Box remotely
date: 2023-10-29 12:00:00 +0200
tags: administration
image: /assets/2023-10-29/title.png
read_more_links:
  - name: Wake me up before you go go - Two cool ways to use Wake-On-Lan to boot a Peakboard Box
    url: /Wake-me-up-before-you-go-go-Two-cool-ways-to-use-Wake-On-Lan-to-boot-a-Peakboard-box.html
downloads:
  - name: ShutdownTimer.pbmx
    url: /assets/2023-10-29/ShutdownTimer.pbmx
---

We often see Peakboard Boxes hanging overhead in large factory halls or warehouses. But there's no need for the boxes to run 24/7. You should turn them off when you don't need them. In this article, we cover three cool ways to shut down a Peakboard box without pulling the plug.

We also have an article covering [how to wake up a Peakboard Box](/Wake-me-up-before-you-go-go-Two-cool-ways-to-use-Wake-On-Lan-to-boot-a-Peakboard-box.html).

## Shutdown Method 1 - Use a timer script

The first shutdown method uses built-in tools from Peakboard Designer. Just place a timer data source in your project, and add a timer script (see screenshot below).

The script is pretty simple: It just checks if the desired shutdown time has passed. For example, if the shift ends at 9 PM, then the script can check if the current time is later than `2130` (which is 9.30 PM).

When the shutdown time is reached, the script sends a shutdown command. This shutdown command is available as a Building Block. But if you prefer pure LUA scripting, you can use `peakboard.shutdown(X)`, where `X` is the number of seconds to wait before actually shutting down the Box.

![image](/assets/2023-10-29/010.png)

In the official Peakboard documentation, you can find an [article](https://help.peakboard.com/scripting/en-quick-tipp-restart.html) that shows a slightly different approach for making a timer-based shutdown script. Depending on your use case, you may prefer this alternative.

## Shutdown Method 2 - Use a global shared function

In this case, we use the same Building Block / LUA function as before. However, we put it into a global function, which is not only global but also shared. This means the Peakboard Box exposes a REST endpoint that can be called from outside.

![image](/assets/2023-10-29/020.png)

![image](/assets/2023-10-29/030.png)

This public endpoint can be called through the web access interface.

![image](/assets/2023-10-29/040.png)

Or it can be implemented by third-party applications that call the endpoint. As you can see in the following screenshot, the endpoint for the function is:

{% highlight url %}
https://<MyBox>:40405/api/functions/ShutMeDown
{% endhighlight %}

You have to provide basic credentials (user and password of the Box). And the call must be an HTTP POST request. HTTP GET is not supported for calling a function.

Also, in the POST request, you have to provide an empty JSON string (`{ }`). Check out the following Postman screenshot to see an example of a valid call.

![image](/assets/2023-10-29/045.png)

These functions can also be used in whatever system landscape / administration or monitoring tool your company already uses. All these tools usually support REST calls natively.

## Shutdown Method 3 - Use a network plug

Some of you might remember the [article about how to use a Shelly plug](/Fun-with-Shelly-Plug-S-Switching-Power-on-and-off.html). That article covers how to monitor power consumption and switch power on / off with a Shelly plug.

It's pretty intuitive to use a Shelly plug to shut down or wake up not only a Peakboard Box, but also the screen. After plugging it in, just use a web browser to shut down the plug, by calling this URL:

{% highlight url %}
http://<MyShellyPlugIP>//relay/0?turn=off
{% endhighlight %}

Needless to say, this method can also be used to switch it on.

If you think that devices like Shelly are not suitable for industrial applications, you might be right. If you prefer a more robust device, look out for helpers like this network plug:

[https://www.reichelt.de/pdu-1-x-schutzkontakt-gude-1105-1-p286109.html](https://www.reichelt.de/pdu-1-x-schutzkontakt-gude-1105-1-p286109.html)

Or, completely omit the network infrastructure and use a remote controlled plug like this:

[https://www.amazon.de/Sonsonai-Sockets-Controls-Decoration-Programmable/dp/B09GLSN1G3/ref=sr_1_1_sspa?crid=3L75E3G7J9NDL](https://www.amazon.de/Sonsonai-Sockets-Controls-Decoration-Programmable/dp/B09GLSN1G3/ref=sr_1_1_sspa?crid=3L75E3G7J9NDL)




