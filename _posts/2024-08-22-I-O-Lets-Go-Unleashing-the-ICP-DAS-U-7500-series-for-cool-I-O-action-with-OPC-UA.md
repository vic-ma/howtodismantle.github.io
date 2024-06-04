---
layout: post
title: I/O, Let's Go - Unleashing the ICP DAS U-7500 series for cool I/O action with OPC UA 
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt
image: /assets/2024-08-22/title.png
read_more_links:
  - name: I/O, Let's go - The hitchiker's guide to I/O devices 
    url: /I-O-Lets-go-The-hitchikers-guide-to-I-O-devices.html
  - name: ICP DAS U-7560 website
    url: https://www.icpdas.com/en/product/U-7560M
  - name: ICP DAS U-7000 Documentation
    url: https://www.icpdas.com/web/product/download/iiot/ua/u-7000/document/user_manual/ua-io_user_manual_en.pdf
downloads:
  - name: ICP-DAS-U-7560-OPC-UA.pbmx
    url: /assets/2024-08-22/ICP-DAS-U-7560-OPC-UA.pbmx
---
In an earlier article we already discussed another I/O module provided by the Taiwan based company ICP DAS: [I/O, Let's Go - Unleashing the ICP DAS ET-2254 with MQTT and Peakboard](/I-O-Lets-Go-Unleashing-the-ICP-DAS-ET-2254-with-MQTT-and-Peakboard.html). In today's article we will discuss another one from the U-7500 series, to be more precise the [U-7560](https://www.icpdas.com/en/product/U-7560M) with 6 digital Inputs and 6 Relay outputs. The big difference is, that it supports OPC UA as the ultimate way of connecting to it. When we use MQTT for accessing I/O modules we always need any kind of MQTT broker in the middle. This is not necessary when using OPC UA; we just connect from Peakboard directly to the I/O device. OPC UA also has some other advantages because it's not necessary to process any kind of JSON stream or whatever the IO module sends via MQTT. In OPC UA always the correct, scalar value is sent and can be processed without further ado.

But there might be also one disadvantage to consider: Usually the OPC UA device comes with a natural limitation of the number of clients. When an Input event should be distributed over more than 4 or 5 clients the OPC UA host could be undersized and this results in unreliable behaviour. In that case MQTT might be the smarter choice.

## Configuring the U-7560



![image](/assets/2024-08-06/010.png)

