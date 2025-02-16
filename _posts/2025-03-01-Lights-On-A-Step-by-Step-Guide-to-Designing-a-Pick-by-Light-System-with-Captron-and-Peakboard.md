---
layout: post
title: Lights On - A Step-by-Step Guide to Designing a Pick-by-Light System with Captron and Peakboard
date: 2023-03-01 03:00:00 +0200
tags: hardware
image: /assets/2025-03-01/title.png
image_header: /assets/2025-03-01/title_landscape.png
read_more_links:
  - name: Captron Pick-by-light
    url: https://captron-solutions.com/en/pick-by-light/
downloads:
  - name: CaptronPBL.pbmx
    url: /assets/2025-03-01/CaptronPBL.pbmx
---
On this blog, we often discuss how Peakboard application can communicate with hardware components that are used in manufacturing and logistics. In today's article, we'll take a detailed look at a pick-by-light system from Captron. Captron is a German manufacturer based in the beautiful state of Bavaria, Southern Germany.

One of the PBL technologies Captron makes is an [LED strip](https://captron-solutions.com/en/pick-by-light_hardware/led-strips/). This strip is mounted onto the racks of manufacturing warehouses and workplaces, to help the workers take goods from the correct warehouse bin. You can see the system in action from the video on the [Captron PBL page](https://captron-solutions.com/en/pick-by-light/).

In this article, we will build a Peakboard application that controls these LED strips. The list of goods to pick is given to the application as a table. As soon as the picking process is confirmed in the application, the corresponding part of the LED strip is switched off. This final process can be seen in the video at the bottom of this page.

## Mounting and configuing the LED strip

The LED strips are mounted on the warehouse racks and wired to a control unit as shown in the picture. The control unit can handle up to four strips.

![image](/assets/2025-03-01/010.png)

As soon as the control unit is connected to the local network, a simple web based configuration page can be accessed through the browser. The only thing we need to configure is the connection to a MQTT broker. And we need to note the so called Unique ID. We will need it later for the MQTT exchange. That's all.

![image](/assets/2025-03-01/020.png)

## Preparing the application

The first data structure we need is the actual order lines as shown in the screenshot. There are typical columns, like Material number, quantity and the warehouse location, or warehouse bin where the goods are stored. Let's assume this data structure is filled from an ERP system. We won't do that here because we discussed that process in many other article (e.g. getting a [transfer order from SAP][/Barcode-Bliss-Part-III-Bringing-ProGlove-and-SAP-together-Transfer-Order-Use-Case.html]). In our sample we just fill this table with random sample data as soon as the user clicks on the "Launch new order" button to keep it as simple as possible. The detais can be seen directly in the [downloadable pbmx](/assets/2025-03-01/CaptronPBL.pbmx).

![image](/assets/2025-03-01/030.png)

The second data structure we need contains the meta data of our warehouse rack. Every warehouse bin has a corresponding LED start and end number. So according to this table we can later switch the light on or off depending on if the warehouse is used in the current order or not. This table must be maintained only once during the set up of the rack. It's also an option to maintain this meta data together with warehouse master data of the ERP system (like SAP or Business Central). But this depends on the use case and the ERP system.

![image](/assets/2025-03-01/040.png)

Our application has only one data source. It's the connection to the MQTT broker. Actually we only need this connection to send out command to switch the lights on off. So using it as a tradional data source is not necessary. However as we need it it anyway, we subscribe to a dedicated MQTT node that represents the health status of the Captron PBL controller. The topic we need to subscribe is "captron.com/SEH200/nd/EU-CravenWealthyFruit/Pub/MAM". The literal "EU-CravenWealthyFruit" refers to the Unique ID we noted earlier from the web interface. The health status is sbumitted in a JSON string that is described in the Captron PBL manual. 

![image](/assets/2025-03-01/050.png)

## Building and sending the light commands

The actual miracle of our application is done in the function SyncLights. The MQTT command is a JSON string that is sent later to the topic "captron.com/SEH200/nd/EU-CravenWealthyFruit/Set/Data/LedStrip". Here we have the Unique ID again. Let's have a look at the JSON.
As we can see we need to provide the information to which of the LED strips the command is sent. In our case it's the first one "LED_STRIP_1". Then we nned to provide the dedicated segments for the lights to be maniuplated. The sample shows the first ten LEDs (StartLED 0 to StopLED 10). The effect 1 means static light, and the color is green. As the "Segments" part is an array, so we can provide multiple of those.

{% highlight json %}
{ "Content": "/Set/Data/LedStrip",
  "LED_STRIP_1": {
    "Active": true,
    "Segments": [
      {
        "StartLED": 0,
        "StopLED": 10,
        "Effect": 1,
        "Colors": [ { "R": 0, "G": 150, "B": 0 } ]
      },
    ] 
} }
{% endhighlight %}

Here's the Building Blocks structure to put the JSON file together:

1. We start with some static MQTT
2. We loop over the order items. As long as the "State" is "A" for "Active", we build a new segment to switch the light on
3. We look up the index of the table entry in the warehouse bin table for the corresponding entry for the bin number
4. The actual segment is built with a placeholder template that replaces start and end LED. The start and LED number is looked up from the WareHouse bin tables according to the bin index.
5. We close the JSON array
6. We send out the JSON command to the MQTT broker

Here's the JSON with the placeholders in it:

{% highlight json %}
{
"StartLED": #[LEDStart]#,
"StopLED": #[LEDEnd]#,
"Effect": 1,
"Colors": [ { "R": 0, "G": 150, "B": 0 } ]
},
{% endhighlight %}

![image](/assets/2025-03-01/060.png)

The last thing we need to add, is the "Done" button. In the list of order items the user can hit "Done" to indicate that the items is picked. So we set the state of the item to "D":

![image](/assets/2025-03-01/070.png)

## result

In the result video we can see the whole picking process. The order is initiated by pressing the button. The three picking items are displayed and the three corresponding warehouse bins are indicated by the LED strip. As soon as picking item is confirmed to be "done" the light is switched off.

{% include youtube.html id="s8Uh0ExfEk8" %}
