---
layout: post
title: Peakboard Meets Shelly - Building a Smart Dashboard for Tracking Temperature and Humidity
date: 2023-03-01 01:00:00 +0200
tags: hardware opcuamqtt
image: /assets/2025-04-02/title.png
image_landscape: /assets/2025-04-02/title_landscape.png
read_more_links:
  - name: Shelly H&T Gen3
    url: https://www.shelly.com/products/shelly-h-t-gen3-matte-black
  - name: Shelly H&T API Documentation
    url: https://shelly-api-docs.shelly.cloud/gen2/Devices/Gen3/ShellyHTG3/
  - name: Shelly H&T Technical Documentation
    url: https://www.shelly.com/blogs/documentation/shelly-h-t-gen3?srsltid=AfmBOop_uRRkBuYODH76QXhhOjD3FCpFxvW4KtqyH2xq85LxG6U4f19C
downloads:
  - name: ShellyHT.pbmx
    url: /assets/2025-04-02/ShellyHT.pbmx
---
More than two years ago, there were already two articles about [Shelly products](): One about [how to use the button](/Building-an-emergency-button-with-Shelly-Button1-and-MQTT.html) and another about [how to use the power plug](/Fun-with-Shelly-Plug-S-Switching-Power-on-and-off.html). Lot's of time has passed since then and many readers came back who actually used the Shelly products even in a professional, industrial environment. Today we want to have a closer look at a relatively new product: The Shelly [Humidity and Temperature sensor](https://www.shelly.com/products/shelly-h-t-gen3-matte-black). It comes with a e-ink display and WiFi connection. How to configure the Wifi connection during initial setup is explained in the [documentation](https://www.shelly.com/blogs/documentation/shelly-h-t-gen3?srsltid=AfmBOop_uRRkBuYODH76QXhhOjD3FCpFxvW4KtqyH2xq85LxG6U4f19C).

In our example we will use MQTT to build a dashbaord with Peakboard for showing the current temperature and humidity as provided by the Shelly sensor. We also will build our own historisation and store the values for later analysis in the Peakboard Hub.

![image](/assets/2025-04-02/010.png)

## Shelly sensor configuration

Our Shelly sensor offers two ways for configuration. It comes with a built-in web interface that can be access during the setup process (see documentation), or the configuration can be also done through the SHelly cloud services, provided that it's configured properly. If it's necessary to avoid any kind of outside connection because of security restrictions within the factory, a cloud connection is NOT mandatory. The screenshot shows the cloud based configuration dialog. The only point we need to configure here is the MQTT connectivity. We use a public MQTT broker, but a private one will do the same. Beside the adress we need to provide a suffix which is used to detemine the MQTT topic.

![image](/assets/2025-04-02/010.png)

The sensor will send out a bunch of MQTT message every couple of minutes. Beside lots of meta and health information we can find temperature and humdity along with a unix time stamp with the exact time of the last measurement. The screenshot shows the MQTT message as subscribed with MQTT explorer.
We must understand that for energy saving purpose the sensor is going sleep between two measurements. It evens discoonnects from WiFi during that sleep time. So we usually can't ping the sensor within the local network. 

![image](/assets/2025-04-02/030.png)

## Building the Peakboard app

On the Peakboard side we will need a MQTT conection. As shwon in the screenshot we must subscribe to three different topics for temperature, humditiy and time:

- MyShellyTemperature/status/humidity:0
- MyShellyTemperature/status/temperature:0
- MyShellyTemperature/status/sys

and then use the data path to find the need value within the JSON formatted message.

![image](/assets/2025-04-02/040.png)

For turning the Unix timestamp into a nicely formatted value we use a data flow along with single line of LUA code for the formatting: os.date("%Y-%m-%d %H:%M:%S", tonumber(item.Unixtime))

![image](/assets/2025-04-02/050.png)

To show the values to the end user we just use tiles, bind it to the source and make sure the data is formatted correctly along with a unit.

![image](/assets/2025-04-02/060.png)

## Historizations

To store the values for future analysis, we will set up a table in [Peakboard Hub](/Peakboard-Hub-Online-An-introduction-for-complete-beginners.html). Just a time stamp and two integer columns fpr Temperature and Humidity.

![image](/assets/2025-04-02/070.png)

In the Peakboard app we set up a data connection to this list and query the last 100 records. We need that as data supply for the chart.

![image](/assets/2025-04-02/080.png)

For the chart we use a simple line chart and connect it directly to the source. In our sample we have two separate charts. One for temperature and one for humditiy.

![image](/assets/2025-04-02/090.png)

The last thing we need to do, is to store the current values on a regular basis into the list. For thta purpose we set up a timer that is triggered every 10 minutes and takes the value from the data flow and just appends in on the list. That's all we need to do.

![image](/assets/2025-04-02/100.png)

## Result

The last screenshot shows the final result in action. The realtime data is displayed from the SHelly H&T sensor and has been transmitted via MQTT. On regular timely basis these values are stored in a Hub list and then queried again from the hub list to be the basis for the value history charts that goes back to the last 100 data points.

![image](/assets/2025-04-02/result.png)