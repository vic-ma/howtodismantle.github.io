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

In the **Module Setting** tab, we can fine tune the 6 digital inputs and 6 relay outputs. The OPC UA description is shown in the OPC UA metadata and can make it a bit easier for OPC UA client to find the right input and output.

![image](/assets/2024-08-22/030.png)

## Set up Peakbard data source

On the Peakboard side, we gain access to the module through the typical OPC UA access. We input the IP address and select anonymous authentication. On the right side, you can see the OPC UA metadata structure. In our case, we subscribe on the `IN0` input and the `IN0_counter` counter. The description we defined for that input (in the web interface) also appears here.

In our example, we assume we're processing the events of a light barrier. So we use the built-in counter as an easy way to get counting functionality. We don't need to implement a counter from scratch.

![image](/assets/2024-08-22/040.png)

The following screenshot shows a test app that displays the state of the input channel and the current value of the counter. The input value has a boolean data type.

![image](/assets/2024-08-22/050.png)

To reset the counter to its predefined initial value (usually 0), we use a Building Block to write to an OPC UA node. The I/O module offers a special node called `ns=2;s=U-7560M.IN0_CounterClear`. If this node is set to `true`, then the counter is reset. To find the ID of a node, we can use the node subscription dialog from the data source windows. All the OPC UA node metadata is shown there.

![image](/assets/2024-08-22/060.png)

## Set the output

Setting an output channel works similarly to resetting a counter: We write to a node. To output 0, we use the node `ns=2;s=U-7560M.RL0`. We set it to 1 to turn the channel on, and to 0 to turn it off.

![image](/assets/2024-08-22/070.png)

## conclusion

Using OPC UA over MQTT is very straight forward because no JSON parsing is necessary. The samples in this article shows that no JSON parsing is necessary. The whole data processing is done type safe without any addtional knowledge about the source. We just use OPC ua metadata to set up out connectivity.  

