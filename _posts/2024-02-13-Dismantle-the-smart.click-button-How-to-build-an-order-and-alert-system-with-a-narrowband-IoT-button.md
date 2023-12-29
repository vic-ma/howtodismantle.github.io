---
layout: post
title: Dismantle the smart.click button - How to build an order and alert system with a narrowband IoT button  
date: 2023-03-01 12:00:00 +0200
tags: api hardware opcuamqtt
image: /assets/2024-02-13/title.png
read_more_links:
  - name: smart.click IoT Button website
    url: https://marketplace.smart.click/
  - name: Narrowband IoT on Wikipedia
    url: https://en.wikipedia.org/wiki/Narrowband_IoT
  - name: IOX Lab
    url: https://ioxlab.de/en/
downloads:
  - name: IOXButton.pbmx
    url: /assets/2024-02-13/IOXButton.pbmx
---

In today's article, we discuss narrowband IoT devices. Narrowband IoT devices are usually not connected to any kind of Wi-Fi or local company network. Instead, they use the cellular network to connect to a server. For technical details, see this Wikpedia article on [Narrowband IoT](https://en.wikipedia.org/wiki/Narrowband_IoT).

The device we look at is called the smart.click button. It's made by a German company called [IOX Lab](https://ioxlab.de/en/). The following picture shows the device. It runs on battery and therefore doesn't need any kind of infrastructure (such as external power or LAN network access) as long as it can connect to a cellphone network.

![image](/assets/2024-02-13/010.jpeg)

We will build a Peakboard application that lets someone call for help by pressing a smart.click button. This button could be placed in an off-site production environment. Or, it could be placed in a local factory where you don't have access to the network infrastructure (for example, if you're a vendor and want to put the button in a client's factory so that they can press it to initiate an order process).

After someone presses the button, a message pops up in the Peakboard application and a timer starts to show how much time has passed since the alarm was triggered. After pressing the button a second time (for example, once the caller receives the order), the alarm is dismissed.

The technology behind this process is MQTT. The IOX button server sends a MQTT message to a broker that is available over the internet, and our Peakboard app subscribes to this MQTT topic. The rest is Peakboard internal logic.

## Set up the smart.click portal

After purchasing a smart.click button, the button will be available in the smart.click portal. We visit the portal to configure what happens after the button is pressed. There are several options, such as SMS, email and MQTT. We select MQTT and define the broker and MQTT topic.

![image](/assets/2024-02-13/020.png)

There are also options to configure the message on the E Ink display of the button. The following screenshot shows the message after the button is pressed and the message is sent.

![image](/assets/2024-02-13/025.png)

And here's how it looks like on the physical button:

![image](/assets/2024-02-13/030.jpeg)

For our example, we use a free, public MQTT broker. In a real-world environment, you would set up your own MQTT broker.

Here's an example message sent by the button. We don't actually use any of the data in the JSON string. We only care about the fact that a message arrives. If you'd like, you can use the attributes from this payload to enhance your project (for example, to display the battery status).

{% highlight json %}
{
  "device": {
    "imei": "867997032398090",
    "imsi": "901288007597541",
    "fw_version": "smartclick_button_2.2.0",
    "battery": 2731,
    "rsrp": -78,
    "cellid": "1203C65",
    "uptime": 1876178520
  },
  "payload": {
    "commands": "ordered"
  }
}
{% endhighlight %}

## Build the Peakboard application

The Peakboard app shows several positions that could show an alarm (K1-4, QM1, QM2). We only show how to build K1, as the others work the same.

Every time an alarm or event is triggered, the corresponding rectangle turns red, and a counter appears, which shows the amount of time that has passed since the event was triggered.

![image](/assets/2024-02-13/040.png)

The data source is pretty straight forward. We configure the same server and topic as in the IOX portal.

![image](/assets/2024-02-13/050.png)

We also need a variable list, as shown in the following screenshot. Each location has one entry there. This list keeps track of which alarms are active and when they were active.

![image](/assets/2024-02-13/060.png)

## Building the scripts

Here's the refreshed script that is triggered every time a message comes in (which is every time the button is pressed). 

We first check if the alarm is currently active.

*If the alarm is inactive*, we must set it to active. So we set the rectangle to red, increase the counter for toady's messages, and set the start time and active flag in the variable list.

*If the alarm is active*, we must set it to inactive. So we set the rectangle color back to normal, and we set the alarm flag to false.

![image](/assets/2024-02-13/070.png)

The second script is a timer script that displays the elapsed time. It is executed every second and generates the wel formatted elapsed time text.

1. We do it only if the alarm is active
2. We translate the start time, that is stored in he variable list, to a real date
3. We substract the alarm datetime from the current datetime and format it correctly to minutes and seconds and apply the result to the corresponding text box.

The actual time calculation (translating it into a datetime first, calculcate and then translate back) is discussed in a different article

![image](/assets/2024-02-13/080.png)

## Result and conclusion

Here's the final resualt how it looks like whe the button is pushed and a message is coming in. You also see the second part of the process when the second message is coming in to switch the alarm off.

![image](/assets/2024-02-13/result.gif)

Please note, that we didn't do anything with the actual payload of the incoming JSon message. Maybe this would be a nice exercise for the reader to monitor the battery status after extracting this information from the JSon string.

