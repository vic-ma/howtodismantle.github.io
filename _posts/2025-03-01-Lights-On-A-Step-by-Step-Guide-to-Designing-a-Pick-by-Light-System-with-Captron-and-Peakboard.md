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

In this article, we will build a Peakboard application that controls these LED strips. Here's how it works:

1. The application takes in a table that contains the list of goods to pick.
2. The application lights up the LED strips next to the items that need to be picked.
3. The worker picks a good and marks it as picked on the application. The application turns off that LED strip.
4. The worker continues picking goods until all LED strips are off.

You can view the complete process in the video at the bottom of this page.

## Mount and configure the LED strip

The LED strips are mounted onto the warehouse racks and wired to a control unit, as shown in the following photo. The control unit can handle up to four strips.

![image](/assets/2025-03-01/010.png)

As soon as the control unit is connected to the local network, a simple web-based configuration page can be accessed through the browser. The only thing we need to configure is the connection to an MQTT broker. We also need to copy the Unique ID. We'll need it later for the MQTT exchange.

![image](/assets/2025-03-01/020.png)

## Prepare the application

The first table we need contains the order lines, as shown in the following screenshot:

![image](/assets/2025-03-01/030.png)

There are the typical columns, like material number, quantity, and the warehouse location or bin where the goods are stored. Let's assume this data structure has been filled by an ERP system. We won't go through that process here, because we've covered it in previous articles (e.g. [getting a transfer order from SAP][/Barcode-Bliss-Part-III-Bringing-ProGlove-and-SAP-together-Transfer-Order-Use-Case.html]).

For our example, we fill this table with random placeholder data as soon as the user clicks on the **Launch new order** button, to keep things as simple as possible. To see the details, check out the [PBMX](/assets/2025-03-01/CaptronPBL.pbmx).

The second table we need contains the metadata of our warehouse rack. Each warehouse bin has a corresponding LED start and end number. So, using this table, we can switch the light on or off, as needed.

After this table is created during the set-up of the rack, it doesn't need to be modified. There's also the option of maintaining this metadata together with warehouse master data of the ERP system (like SAP or Business Central). But this depends on your use case and your ERP system.

![image](/assets/2025-03-01/040.png)

Our application has only one data source, which is the connection to the MQTT broker. We need this connection to send the command to switch the lights on and off. So, we subscribe to a dedicated MQTT node that represents the health status of the Captron PBL controller. This is the topic we subscribe to:

{% highlight url %}
captron.com/SEH200/nd/EU-CravenWealthyFruit/Pub/MAM
{% endhighlight %}

The literal `EU-CravenWealthyFruit` is the Unique ID that we copied from the web interface. The health status is submitted in a JSON string, as described in the Captron PBL manual. 

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
