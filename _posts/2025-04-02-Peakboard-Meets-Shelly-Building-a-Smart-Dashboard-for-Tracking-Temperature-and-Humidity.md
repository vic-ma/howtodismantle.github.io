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
Over two years ago, we published two articles about Shelly products: One about [how to use the button](/Building-an-emergency-button-with-Shelly-Button1-and-MQTT.html) and another about [how to use the power plug](/Fun-with-Shelly-Plug-S-Switching-Power-on-and-off.html). A lot of time has passed since then and many readers have come back and have actually used Shelly products in a professional, industrial environment.

Today we'll take a closer look at a relatively new product: The Shelly [Humidity and Temperature sensor](https://www.shelly.com/products/shelly-h-t-gen3-matte-black). It comes with an e-ink display and Wi-Fi connection. To learn how to configure the Wi-Fi connection during the initial setup, see the [official documentation](https://www.shelly.com/blogs/documentation/shelly-h-t-gen3?srsltid=AfmBOop_uRRkBuYODH76QXhhOjD3FCpFxvW4KtqyH2xq85LxG6U4f19C).

In our example we'll use MQTT to build a dashboard with Peakboard for showing the current temperature and humidity, as provided by the Shelly sensor. We'll also build our own historization and store the values for later analysis in Peakboard Hub.

![image](/assets/2025-04-02/010.png)

## Shelly sensor configuration

Our Shelly sensor offers two options for configuration. It comes with a built-in web interface that can be accessed during the setup process (see the official documentation). The configuration can be also done through the Shelly cloud services, provided that it's configured properly. However, if it's necessary to avoid any kind of outside connection due to security restrictions within the factory, then a cloud connection can be avoided.

The following screenshot shows the cloud-based configuration dialog. The only thing we need to configure here is the MQTT connectivity. We use a public MQTT broker, but a private one will do the same. Beside the address, we also need to provide a suffix, which is used to determine the MQTT topic.

![image](/assets/2025-04-02/020.png)

The sensor will send a bunch of MQTT messages every couple of minutes. Besides metadata and health information, we can also find the temperature and humidity along with a Unix time stamp with the exact time of the last measurement. The following screenshot shows the MQTT message, as subscribed with MQTT explorer.

It's important to note that for energy saving purposes, the sensor goes to sleep between the two measurements. It evens disconnects from Wi-Fi during that sleep time. So we usually can't ping the sensor within the local network. 

![image](/assets/2025-04-02/030.png)

## Build the Peakboard app

On the Peakboard side, we need an MQTT connection. As shown in the following screenshot, we subscribe to three different topics: temperature, humidity, and time.

- `MyShellyTemperature/status/humidity:0`
- `MyShellyTemperature/status/temperature:0`
- `MyShellyTemperature/status/sys`

We then use the data path to find the desired value within the JSON message.

![image](/assets/2025-04-02/040.png)

In order to turn the Unix timestamp into a properly formatted value, we use a data flow, along with single line of LUA code:

{% highlight lua %}
os.date("%Y-%m-%d %H:%M:%S", tonumber(item.Unixtime))
{% endhighlight %}

![image](/assets/2025-04-02/050.png)

To show the values to the end user we just use tiles, bind it to the source and make sure the data is formatted correctly along with a unit.

![image](/assets/2025-04-02/060.png)

## Historization

To store the values for future analysis, we will set up a table in [Peakboard Hub](/Peakboard-Hub-Online-An-introduction-for-complete-beginners.html). Just a time stamp and two integer columns for temperature and humidity.

![image](/assets/2025-04-02/070.png)

In the Peakboard app we set up a data connection to this list and query the last 100 records. We need that as data supply for the chart.

![image](/assets/2025-04-02/080.png)

For the chart we use a simple line chart and connect it directly to the source. In our sample we have two separate charts. One for temperature and one for humditiy.

![image](/assets/2025-04-02/090.png)

The last thing we need to do, is to store the current values on a regular basis into the list. For that purpose we set up a timer that is triggered every 10 minutes and takes the value from the data flow and just appends in on the list. That's all we need to do.

![image](/assets/2025-04-02/100.png)

## Result

The last screenshot shows the final result in action. The realtime data is displayed from the SHelly H&T sensor and has been transmitted via MQTT. On regular timely basis these values are stored in a Hub list and then queried again from the hub list to be the basis for the value history charts that goes back to the last 100 data points.

![image](/assets/2025-04-02/result.png)