---
layout: post
title: Fun with Shelly Plug S - Switching Power on and off
date: 2023-02-09 12:00:00 +0200
tags: hardware
image: /assets/2023-02-09/title.jpg
read_more_links:
  - name: Shelly Plug S at Amazon
    url: https://www.amazon.de/dp/B093PW6JK6?psc=1&ref=ppx_yo2ov_dt_b_product_details
  - name: Shelly Plug S API documentation
    url: https://shelly-api-docs.shelly.cloud/gen1/#shelly-button1-overview
downloads:
  - name: ShellyPlug.pbmx
    url: /assets/2023-02-09/ShellyPlug.pbmx
---
[Shelly](https://www.shelly.cloud/) is a Germany and US based company that provides gadgets for home automation. Their products are popular among home automation enthusiasts, because unlike competing products, you can easily access Shelly's products with standard technologies like REST and MQTT. Other vendors desperately try to sell their own landscape, app, cloud, etc., and while the Shelly guys have those things too, they don't build a fence around their cloud or products.
As already mentioned, Shelly is built for private use at home. Therefore, we should carefully check if their products meet the reader's standards before using them in a professional, industrial environment.

In this article, we will build an app that sends commands to the Shelly Plug S to switch a light on and off. We will use a toggle button on the Peakboard screen so that when the app starts, it should already have the correct state representing whether the light is on or off. We will also show the curent power consumption of the light with a gauge.
Please note: This will be a direct communication between Peakboard and Shelly—no MQTT, no hub, no cloud....

## The API of the Shelly Plug S

Please refer to the [official documentation](https://shelly-api-docs.shelly.cloud/gen1/#shelly-button1-overviewhttps://www.shelly.cloud/documents/user_guide/shelly_button_1.pdf) for how to use the API of the Plug. Here are the three function we will use.

{% highlight url %}
http://<MyShellyPlugIP>/status
{% endhighlight %}

returns a JSON object with lots of information about the plug. For our needs, there are two attributes: `meters[0].power` is the current power consumption and `ison` indicates whether the plug is turned on or off.
The two functions

{% highlight url %}
http://<MyShellyPlugIP>//relay/0?turn=on
http://<MyShellyPlugIP>//relay/0?turn=off
{% endhighlight %}

are used to send commands to turn the power on and off.

## Preparing the canvas

We prepare the canvas with a gauge; a big, fat toggle button; and a text box that prints out the formatted power consumption value.

![image](/assets/2023-02-09/010.png)

## Setting the initial state of the toggle button

We start with a simple JSON data source that points to the status API.

![image](/assets/2023-02-09/020.png)

This data source only runs once to set the state of the toggle button. So we add a simple command to the _Refreshed_ script of this data source, to set the _active_ attribute of the toggle button.

![image](/assets/2023-02-09/030.png)

## Processing the power consumption

The second data source is executed once per second and calls the same API as the last one. However, we use a path to go deeper into the JSON result in order to read the power consumption according to the API documentation.

![image](/assets/2023-02-09/040.png)

Before we can use the power value, we need to re-format it from a string to a number. A small data flow will do that for us:

![image](/assets/2023-02-09/060.png)

And now we can bind the text box and gauge to the output of the dataflow:

![image](/assets/2023-02-09/050.png)

## Sending on / off commands

For sending the on / off commands, we prepare two simple JSON sources that point to the two corresponding API calls. As we don't want to fire them without anyone pressing the toggle button, we set both to not enabled.

![image](/assets/2023-02-09/070.png)

Finally, the toggle button has two events that are relevant for us. One for checking, and one for unchecking. In both, we just reload the JSON source for switching on / off:

![image](/assets/2023-02-09/080.png)

## The result

Here's how our final application works in real life. You can see in the video how the power consumption goes up when the light is switched on. Feel free to [download](/assets/2023-02-09/ShellyPlug.pbmx) it.

{% include youtube.html id="n9s4w6S71KU" %}


