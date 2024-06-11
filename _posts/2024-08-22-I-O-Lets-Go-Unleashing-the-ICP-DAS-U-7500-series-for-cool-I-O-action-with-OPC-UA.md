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
In a [previous article](/I-O-Lets-Go-Unleashing-the-ICP-DAS-ET-2254-with-MQTT-and-Peakboard.html), we discussed an I/O module provided by the Taiwan-based company ICP DAS. In today's article, we will discuss another I/O module from the U-7500 series: the [U-7560](https://www.icpdas.com/en/product/U-7560M) with 6 digital Inputs and 6 Relay outputs.

The main difference is that it supports OPC UA as the primary way of connecting to it. When we use MQTT to access I/O modules, we always need some kind of MQTT broker in the middle.

This is not necessary when using OPC UA; we just connect Peakboard directly to the I/O device. OPC UA also doesn't need to process any kind of JSON stream or whatever the I/O module sends via MQTT. In OPC UA, the correct, scalar value is always sent and can be used without further processing.

But there is one disadvantage to consider: The OPC UA device usually comes with a natural limit for the number of clients. When an input event needs to be distributed over more than 4 or 5 clients, the OPC UA host could be undersized. This results in unreliable behavior. In this case, MQTT might be the smarter choice.

![image](/assets/2024-08-22/010.png)

## Configure the U-7560

The U-7560 comes with a web interface, like other ICP DAS products. Because we want to focus on OPC UA, we need to make sure that OPC UA access is enabled. The following screenshot shows that you can enable anonymous login, certificate login, and user/password login:

![image](/assets/2024-08-22/020.png)

In the **Module Setting** tab, we can fine tune the 6 digital inputs and 6 relay outputs. The **OPC UA Description** is shown in the OPC UA metadata and can make it a bit easier for OPC UA client to find the right input and output.

![image](/assets/2024-08-22/030.png)

## Setting up Peakbard data source

On the Peakboard side access to the module is easily done through the typical OPC UA access. We just use the IP address and choose access without any certificates or password. On the right side we can see the OPC UA metadata structure. In our case we subscribe on the Input IN0 and also on the counter IN0_counter. The description we defined in the web interface for that input channel also comes up here.

As in our sample we assume to process the events of a light barrier. So using the built-in-counter functionality is an easy-to-use feature for counting. So we don't need to implement it on our own.

![image](/assets/2024-08-22/040.png)

The screenshot shows a test app that just displays the state of the Input channel and the current value of the counter. The input value has a boolean data type.

![image](/assets/2024-08-22/050.png)

To reset the counter to its predefined initial value (usually just 0) we make use of a Building Block to write to an OPC UA node. The I/O module offers a special node called "ns=2;s=U-7560M.IN0_CounterClear". If this node is set to "true" the counter is reset. To find out the ID of a node we can just use the node subscription dialog from the data source windows. All the OPC UA node metadata is shown there.

![image](/assets/2024-08-22/060.png)

## Setting the output

Setting an output channel works similiar like resetting a counter. We just write to a node. For output 0 the node is called "ns=2;s=U-7560M.RL0" and should be set to "1" to turn the channel on, and to "0" to turn it off.

![image](/assets/2024-08-22/070.png)

## conclusion

Using OPC UA over MQTT is very straight forward because no JSON parsing is necessary. The samples in this article shows that no JSON parsing is necessary. The whole data processing is done type safe without any addtional knowledge about the source. We just use OPC ua metadata to set up out connectivity.  

