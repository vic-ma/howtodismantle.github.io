---
layout: post
title: Barcode Bliss - Part I - Integrating ProGlove Scanners with Peakboard
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt
image: /assets/2024-08-14/title.png
read_more_links:
  - name: Barcode Bliss - Part II - Sending Feedback to ProGlove Scanners
    url: /2024-08-30-Barcode-Bliss-Part-II-Sending-Feedback-to-ProGlove-Scanners.html
  - name: ProGlove Documentation
    url: https://docs.proglove.com/en/connect-gateway-to-your-network-using-mqtt-integration.html
downloads:
  - name: ProGloveTestCenter.pbmx
    url: /assets/2024-08-14/ProGloveTestCenter.pbmx
---
ProGlove is new generation of barcode scanners integrated in a real glove which gives the user the easy way to scan a barcode without having to take a barcode scanner first and then put it back to the station after scan. The scanner is just always "at hand" and can be used right away. For all the reader who never saw it before, here's how it looks like:

![image](/assets/2024-08-14/010.png)

In this article we will cover the question how to configure the scanner and discuss ways to integrate the scan event in a Peakboard application. In the second part of this mini series, we go one step further and give the barcode scanner user feedback about the scan, e.g. if the scanned code was processed succesfully. The feedback can be either a simple green or red light or even a small display on the scanner.
The second article can be found [here](/2024-08-30-Barcode-Bliss-Part-II-Sending-Feedback-to-ProGlove-Scanners.html).

## Configuration

The scanner comes with some kind of base station called gateway. The easiest way to configure both the scanner and the gateway is to use the [PrGlove Insight portal](https://insight.proglove.com/). The portal user can register their device in the portal, then take the necessary steps to configure the gateway or the device and apply the new configuration to the gateway by scanning a large barcode that holds the configuration metadata or download the confirguation file and then apply it to the gateway by connecting the gateway to be used as a mass storage of the PC. The datails can be found in the [documentation](https://docs.proglove.com/?lang=en).

Bascially there are two useful operating modes that can be used together with Peakboard:

1. The USB mode. In that case the gateway is connected to the USB port of the Peakboard box. If we have only one gateway and don't need to use multiple scanners and other events then just the scanned code, this is the way to go.
2. The MQTT mode. In the case the bidirectional communication is done through an MQTT server (on prem or in the cloud, any is fine). if we want to use the full ProGlove power to give the enduser feedback for the scan or use other events, that's the way to go.

## The USB mode

The USB mode is quite simple to apply. The screenshot shows the configuration in the Insight portal. We just switch it on. That's it.

![image](/assets/2024-08-14/020.png)

Now we can just plug the gateway directly into the USB port.

![image](/assets/2024-08-14/030.jpg)

In the Peakboard designer we just use the global event "KeyInput" and then scanned code is available through the "get string parameter" block. Every time a code is scanned this event is triggered.

![image](/assets/2024-08-14/040.png)

## The MQTT mode

The MQTT mode is much more fun than USB. The configuration is also not too complicated. We just set the broker and alsothe main topic.That's it.

![image](/assets/2024-08-14/050.png)

Every time a code is scanned the gateway sends a JSON string to the MQTT broker. The JSON string looks like this. Beside the scanned code we can also see some more or less useful attributes related to the scan.

{% highlight json %}
{
    "api_version":"1.0",
    "event_type":"scan",
    "event_id":"22d4a950-b373-42a3-8428-7a3887e78583",
    "time_created":1699621609245,
    "scan_code":"YVQGZZNSXU",
    "device_serial":"MDMR102104863"
}
{% endhighlight %}

Let's switch to the Peakboard side. We set the MQTT broker and configure two subscriptions. Both listen at the topic "Peakboard/gateway/PGGW402650394/scan". This topic is built from the configured main topic plus "gateway" plus the serial number of the gateway plus the event "scan".

The Peakboard data source gives us the option to process the json right away by using so called data paths. We know from the sample JSON the scanned code can be found within the JSON under "scan_code", while the serial number is at "device_serial". And so we configure the two subscriptions that the JSON is translated into the scalar values we want to process later.

![image](/assets/2024-08-14/060.png)

The last thing we do is to preapre some text boxes to show the scanned code through data binding:

![image](/assets/2024-08-14/070.png)

If we want react directly to a scan we ideally put out script or Building Blocks into he refreshed event of this data source. It's triggered everytime a scan is coming in on the MQTT topic.

## conclusion and result

Building a Peakboard app together with ProGlove scanners is straight forward. No matter if you use USB or MQTT mode. The video show the simple app and displays the scanned code in the bound text boxes.

{% include youtube.html id="tMobjoShVS0" %}
