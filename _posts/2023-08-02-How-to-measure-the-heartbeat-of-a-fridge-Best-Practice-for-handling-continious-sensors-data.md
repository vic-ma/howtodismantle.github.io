---
layout: post
title: How to measure the heartbeat of a fridge - Best Practice for handling continious sensors data
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2023-08-02/title.jpg
read_more_links:
  - name: Fun with Shelly Plug S - Switching Power on and off
    url: /Fun-with-Shelly-Plug-S-Switching-Power-on-and-off.html
  - name: Shelly Plug S at Amazon
    url: https://www.amazon.de/dp/B093PW6JK6?psc=1&ref=ppx_yo2ov_dt_b_product_details
  - name: Shelly Plug S API documentation
    url: https://shelly-api-docs.shelly.cloud/gen1/#shelly-button1-overview
downloads:
  - name: FridgePowerConsumptionMonitor.pbmx
    url: /assets/2023-08-02/FridgePowerConsumptionMonitor.pbmx
---
A regular fridge offers an exciting possibility to get insights about the power consumption of house hold appliances. Today we will look on how to measure and handle this power consumption in Peakboard. For the actual measure we use the same Shelly plug we already discussed back a couple of months ago. Please make sure to have read and understood [this article](Fun-with-Shelly-Plug-S-Switching-Power-on-and-off.html) first, because some basic knowledge about Shelly and its API is minimum requirement to understand of what we do today.

## Preparing and filling the data structure

The actual data source is a JSon call to the Shelly status API that is performed every 5 seconds. Please check the old article mentioned above for details. The refresh rate is 5 seconds.

![image](/assets/2023-08-02/005.png)

For the visualisation we need to store the data in a local table. To be more precise, we use a table-like variable. The table only has two columns: _TS_ (string) is the time stamp, _Power_ (number) is the current power value. We also tick the _Store variable on box_ setting. So when the box is restarted, the last data is still there and won't get lost.

![image](/assets/2023-08-02/010.png)

The next thing we need is just a regular time source to have the current time value available when we process a data point later.

![image](/assets/2023-08-02/020.png)

Here's the most important part. The _Refreshed_ script of the JSon data source. It basically consits of two major steps:

1.) Add the latest data point to our local table by using the _power_ element of the JSon output and the timestamp provided by the time source.
2.) Check if the number of elements in the table. If the number exceeds 1000 the first element (shoich is the oldest) will be deleted.

![image](/assets/2023-08-02/030.png)

With this kind of pattern, the local table will always have the last 1000 data points. With a refresh rate of 5 seconds, the table will hold 1000 x 5 = 5000 sec = roughly 1,5 hours of data. So we can visualize what happened in he last 1,5 hours.

## Visualizing the data

For the visualizing part we just use a regular area chart with the power consumption on the y axis. On the right side we see a small table of the last 20 data points and a tile with the current value. Feel free to download the [pbmx](/assets/2023-08-02/FridgePowerConsumptionMonitor.pbmx) to find out more more details.

![image](/assets/2023-08-02/040.png)

## The result

Here you can see the result of typical LG fridge with two doors. The periods of cooling (with high power consumption) and perdiods when the fridge is actually idle can be seen clearly. Also we see, that the power consumption varies within a cooling period which is probably related to internal processes. Initially there's more power needed to get the compressor in efficient working mode. Also we can see, that the power consumption goes up for a couple of seconds in form of a small peak needle before it breaks down to idle. Can anyone explain this?

![image](/assets/2023-08-02/050.png)

