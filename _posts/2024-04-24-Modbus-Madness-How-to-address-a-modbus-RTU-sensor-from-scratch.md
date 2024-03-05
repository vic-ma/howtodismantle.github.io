---
layout: post
title: Modbus Madness - How to address a Modbus RTU sensor from scratch  
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt
image: /assets/2024-04-24/title.png
read_more_links:
  - name: Modbus on Wikipedia
    url: https://en.wikipedia.org/wiki/Modbus
  - name: TCP Gateway Serial to Ethernet Converter
    url: https://www.amazon.de/dp/B0BR4RX4CX?psc=1&ref=ppx_yo2ov_dt_b_product_details
  - name: Temperature and Humidity Transmitter
    url: https://www.amazon.de/dp/B09FPSHXD5?psc=1&ref=ppx_yo2ov_dt_b_product_details
downloads:
  - name: ModbusHumidityAndTemperature.pbmx
    url: /assets/2024-04-24/ModbusHumidityAndTemperature.pbmx
---
Modbus is a standardized protocol for addressing sensors. It was invented by Schneider Electric back in the late 70s, but is still widely used. In this article, you will learn how to use the Peakboard Modbus data source to address a sensor, get the values, and do something useful with it.

## Basic ideas behind Modbus

The idea is to integrate several sensors in a bus system where every sensor has an address. Every time the master (like a PLC) needs to query a value from the sensors it sends a request package with the address of the sensor into the bus and the sensor sends back his answer. Depending on the nature of the answer there are four main "structures" how to organize the data exchange. A discrete output or input, a so called "Holding Register" for command and configuration values, and the "Input Register" which contains the actual measures or values of the sensor. As we want to query sensor values we go for the "Input Register" here in our sample. Please also check out [Wikipedia](https://en.wikipedia.org/wiki/Modbus) to learn more about the protocol details.

## RTU and TCP and the hardware we need

Most Modbus sensors are so called RTU sensors which offer just a serial communication. This standard is called RS485. For our sample we use a [Jiaminye Temperature and Humidity Transmitter Rs485 Serial Communication Temperature Sensor](https://www.amazon.de/dp/B09FPSHXD5?psc=1&ref=ppx_yo2ov_dt_b_product_details). The image show how this sensors looks like, beside two power lines there are two communication lines.

![image](/assets/2024-04-24/010.jpeg)

Now the problem is, that Peakboard ideally works in a world where you can just plug into a modern network and don't need to deal with single cables and a serial communication from the 80s. That's why we need a second piece of hardware. A so called [Modbus RTU to Modbus TCP Gateway Serial to Ethernet Converter]() that translates the serial communication to something that can be addressed by using a modern LAN cable. The actual protocol stays the same, it's just a physical converter. We usually use the term "Modbus RTU" for he old style and "Modbus TCP" for the new style. Peakboard only supports "Modbus TCP" and this RTU/TCP converter helps to make any Modbus sensor available and addressable.

![image](/assets/2024-04-24/020.jpeg)

So again: We mount the sensor to the converter and the converter is available in the same LAN as the Peakboard box (and the designer of course).

## The design side / Configuring the data source

After having mastered the physical connection the configuration of the data source is not too complicated. We need to provide the IP address of the RTU/TCP converter. And also the the Bus address of our sensor within the Modbus bus system (Unit ID). In our case this is just "1". We also define that we want to read multiple input registers and read two data points. Ideally the preview works right away (see right side) and we can already see the temperature and humidity data. The temperature is displayed in tenth of degrees and the humidity in tenth of perecetage.

![image](/assets/2024-04-24/030.png)

The next screenshot just shows how to format the numbers correctly. No need for any scripting. We just use the standard formatting options to adjust the multiplier, the measure unit and the number of decimal digits. That's it.

![image](/assets/2024-04-24/040.png)

## result and conclusion

As you see in the article, mastering Modbus with both TCP and RTU is actually not too difficult as long as you master the hardware side and have at least basic knowledge about how Modbus works and even more important: How the sensor actaully works in the context of the serial communication. Unfortunately there's no way around reading each sensor's documentation. There's no standard in how the values can be addressed and interrpreted. Every vendor is implenting their own idea. 

![image](/assets/2024-04-24/050.png)