---
layout: post
title: Bed time stories - Three ways to shut down a Peakboard box remotely
date: 2023-03-01 12:00:00 +0200
tags: administration
image: /assets/2023-10-29/title.png
read_more_links:
  - name: Wake me up before you go go - Two cool ways to use Wake-On-Lan to boot a Peakboard box
    url: /Wake-me-up-before-you-go-go-Two-cool-ways-to-use-Wake-On-Lan-to-boot-a-Peakboard-box.html
---

We often see Peakboard boxes hanging overhead in large factory halls or warehouses. There's no need to let them run 24/7. Turn them off if you don't need them. In this article we cover three cool ways to shut down a Peakboard box without pulling plug. How to wake it up again is covered [here](/Wake-me-up-before-you-go-go-Two-cool-ways-to-use-Wake-On-Lan-to-boot-a-Peakboard-box.html).

## Shutdown Method 1 - use a timer script

The first shutdown method just uses common tools within the Peakboard designer. Just place a timer data source in your project, and then add a time script (see screenshot). The script itself is super simpel. Just check if the time is later then a certain point in time (e.g. when the shift ends at 9PM, just check for 2130 (which is 9.30PM)). When the time is reached, just send a shutdown command, which is available as a Building Block. If you prefer pure LUA scripting the command is peakboard.shutdown(X), while X is the number of seconds until the shut down is executed. 

![image](/assets/2023-10-29/010.png)

## Shutdown Method 2 - Use a global shared function

In that case we use the same Building Block / LUA function as discussed earlier. However we put it into a global function, which is not only global but also shared. This means the Peakboard box exposes an endpoint. 

![image](/assets/2023-10-29/020.png)

![image](/assets/2023-10-29/030.png)

This public endpoint can be either called throu web access...

![image](/assets/2023-10-29/040.png)

... or can be easily implemented all kinds of application by just calling a rest service. As you see in this screenshot the URL of your function looke like https://<MyBox>:40405/api/functions/ShutMeDown. You have to provide a basic authentification (user and password of the box). And the call must be a htttp-POST. GET is not supported to call a function. And in the POST statement you have to provide an empty JSon string represented by "{ }". Check out he following screenshot of Postman on how to call the function successfully.

![image](/assets/2023-10-29/045.png)

Please note, that these kind of functions can be also used in any kind of system landscape / administration or monitoring tool, whatever is already used in the company's infrastructure. All these tools usually support such rest call natively.

## Shutdown Method 3 - Use a network plug

Some of you might remember the [article about how to use a Shelly plug](https://how-to-dismantle-a-peakboard-box.com/Fun-with-Shelly-Plug-S-Switching-Power-on-and-off.html) to monitor power consumption and switch power on / off. It's pretty obvious to use such a Shelly plug to shut down or wake up not only the box, but also the screen. After plugging it in, just use a web browser to shut down the plug calling this URL:

{% highlight url %}
http://<MyShellyPlugIP>//relay/0?turn=off
{% endhighlight %}

Useless to say that this method can be also used to switch it on.

If you think, that such gear like Shelly is not suitable for industrial applications you might be right. If you prefer a more solid gear look out for helpers like this network plug:

[https://www.reichelt.de/pdu-1-x-schutzkontakt-gude-1105-1-p286109.html](https://www.reichelt.de/pdu-1-x-schutzkontakt-gude-1105-1-p286109.html)

Or completely omit the network infrastructure and use remote controlled plug like this:

[https://www.amazon.de/Sonsonai-Sockets-Controls-Decoration-Programmable/dp/B09GLSN1G3/ref=sr_1_1_sspa?crid=3L75E3G7J9NDL](https://www.amazon.de/Sonsonai-Sockets-Controls-Decoration-Programmable/dp/B09GLSN1G3/ref=sr_1_1_sspa?crid=3L75E3G7J9NDL)




