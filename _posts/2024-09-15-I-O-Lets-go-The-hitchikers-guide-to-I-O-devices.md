---
layout: post
title: I/O, Let's go - The hitchiker's guide to I/O devices 
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt api
image: /assets/2024-09-15/title.png
read_more_links:
  - name: Unleashing the ICP DAS ET-2254 with MQTT and Peakboard
    url: /I-O-Lets-Go-Unleashing-the-ICP-DAS-ET-2254-with-MQTT-and-Peakboard.html
---
In this blog we already talked about various devices to be connected with Peakboard and build beautiful applictions. One family of devices are I/O devices. These are used to connect any kind of sensors like light barriers, buttons, temperature sensors, or countlesse others. On the output side we often see actors like traffic lights or sound alarms.

On the market there are literally hundreds of these I/O devices and the overall majority of those work very well with Peakboard. This might be confusing for customers who just started their journey and still try to figure out on which brand or device to build their architecture. In his article we will discuss the five typical ways to connect to an I/O device and then introduce four I/O devices along with their features.

## Techical connectivity

There are uncountable ways to connect to I/O devices, however these five are mostly seen among the Peakboard customers.

1. OPC UA. The I/O device exposes an OPC UA endpoint and Peakboard connects as a client to this OPC UA server. Depending on the device the number of Peakboard clients might be limited to 3 or 5.
2. MQTT. The I/O device is connected to an MQTT broker and the Peakboard application also connectes to this broker. The bi-directional dataflow is established by exchnanging messages throught MQTT topics.
3. Modbus. The I/O device offers a modbus TCP endpoint and can be addressed by using the Modbus data source within the Peakboard application.
4. REST API. The I/O device offers some kind of REST API endpoint that can be used by using the JSON data source.
5. PLC. The I/O device is actually not directly connected to the network, but connected to a PLC. In that case the Peakboard application talks to the PLC (could be Siemens, Mitsubishi, Rockwell or any other). This way only makes sense when the Peakboard app already uses this PLC connection for other activities.

### Advantech WISE-4012

The Advantech WISE-4012 comes with 4 analog and 4 digital inputs, and also 2 digital outputs. It supports MQTT, REST API and Modbus access. It's meant to be only used with Wifi, so no LAN plug available.

Estimated cost: 160 EUR

[Click here to jump directly to the vendor](https://buy.advantech.eu/SRP-IoT-Gateways/IoT-Gateways-Devices-IoT-Wireless-I-O/model-WISE-4012-AE.htm?gad_source=1&gclid=Cj0KCQjw6uWyBhD1ARIsAIMcADqrDuho0MeX74GCqbDntv0ghvNkM322MBmhCc3lf9Dh2iv-wFQGe7AaAl6ZEALw_wcB)

![image](/assets/2024-09-15/010.png)

### ICP DAS ET-2254

The ET-2254 is perfect for wired networks and offers 16 I/O ports that can be freely defined as Input or Output. Access is possible through MQTT and Modbus.

Estimated cost: 173 EUR

An addtional article is available here: [Unleashing the ICP DAS ET-2254 with MQTT and Peakboard](/I-O-Lets-Go-Unleashing-the-ICP-DAS-ET-2254-with-MQTT-and-Peakboard.html)

[Click here to jump directly to the vendor](https://www.icpdas.com/en/product/ET-2254)

![image](/assets/2024-09-15/020.png)

### ICP DAS U-7560M

The U-7500 series comes with an OPC UA interface for supereasy connectivity and brings 6 inputs and 6 outputs. It also supports MQTT but when OPC UA is available prefererring OPC UA over MQTT is a smart choice.

Estimated cost: 176 EUR

[Click here to jump directly to the vendor](https://icpdas-europe.com/Remote-I-O-Module/MQTT/U-7560M-CR)

![image](/assets/2024-09-15/030.png)

### Pepperl+Fuchs IO-Link-Master ICE2-8IOL-G65L-V1D

This device supports connecting IO-Link devices and sensors. The configuration via web interface is very easy.
For the connectivity we can choose between OPC, MQTT and Modbus. The nice thing with OPC UA is, that the attribtues of the connected IO-Link-device are reflected in the node structure which makes it super easy to connect and find the the right values during design time. The downside is, that IO-link related devices are usually much more expensive.

Estimated cost: 700-900 EUR

[Click here to jump directly to the vendor](https://www.pepperl-fuchs.com/germany/de/classid_4996.htm?view=productdetails&prodid=96749
)

![image](/assets/2024-09-15/040.jpg)

## Overview


|  | WISE-4012​ | ET-2254​​ | U-7560M CR​​ | IO-Link​​ |
| ------------- | ------------- |
| WLAN​  | X |  |  |  |
| Ethernet​  |  | X | X | X |
| OPC-UA​  |  |  | X | X |
| MQTT​  | X | X |  | X |
| Modbus​  |  | X |  | X |
| Digital in/out​  | X | X | X | X |
| Analog in/out​  | X | X | X | X |
| IO-Link  |  |  |  | X |


