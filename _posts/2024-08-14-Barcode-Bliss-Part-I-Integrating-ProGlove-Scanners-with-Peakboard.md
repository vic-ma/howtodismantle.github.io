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
    url: https://docs.proglove.com/en/connect-Gateway-to-your-network-using-mqtt-integration.html
downloads:
  - name: ProGloveTestCenter.pbmx
    url: /assets/2024-08-14/ProGloveTestCenter.pbmx
---
ProGlove creates next-generation barcode scanners that are integrated into a real glove, so the user can scan barcodes without having to pick up and put down a conventional barcode scanner. The scanner is always "at hand," and can be used at any time. For readers who've seen a ProGlove scanner before, here's what it looks like:

![image](/assets/2024-08-14/010.png)

In this article, we will discuss how to configure the scanner and integrate the scan event into a Peakboard application.

In the [second part of this mini-series]((/2024-08-30-Barcode-Bliss-Part-II-Sending-Feedback-to-ProGlove-Scanners.html)), we will go one step further and give the barcode scanner user feedback about the scan, like if the scanned code was processed successfully. The feedback can be given by a simple green or red light or even a small display on the scanner.

## Configuration

The scanner comes with a base station called a Gateway. The easiest way to configure both the scanner and the Gateway is to use the [ProGlove Insight portal](https://insight.proglove.com/).

You register your device in the portal, then configure it and apply the new configuration to the Gateway. You can do this by scanning a large barcode that holds the configuration information, or by downloading the configuration file and applying it to the Gateway by connecting the Gateway to your computer as mass storage. See the [ProGlove documentation](https://docs.proglove.com/?lang=en) for more information.

There are two useful operating modes that can be used with Peakboard:

1. **The USB mode.** In this mode, the Gateway is connected to the USB port of the Peakboard box. If you only have one Gateway and don't need to use multiple scanners, and you only need the code scanning event, then this is the way to go.
2. **The MQTT mode.** In this case, the bidirectional communication is done through an MQTT server (on prem or in the cloud; either is fine). If you want to use the full power of ProGlove and give the user feedback about their scans, or use other events, then this is the way to go.

## The USB mode

The USB mode is simple to apply. The following screenshot shows the configuration in the Insight portal. We just need to switch it on. That's it.

![image](/assets/2024-08-14/020.png)

Now, we can plug the Gateway directly into the USB port of our Peakboard Box:

![image](/assets/2024-08-14/030.jpg)

In Peakboard Designer, we use the global event **KeyInput**. We can get the scanned code through the **get string parameter** block. Each time a code is scanned, this event gets triggered.

![image](/assets/2024-08-14/040.png)

## The MQTT mode

The MQTT mode is much more fun than the USB mode. The configuration is also not too complicated. We just need to set the broker and main topic. That's it.

![image](/assets/2024-08-14/050.png)

Every time a code is scanned, the Gateway sends a JSON string to the MQTT broker. The following is an example of a JSON string. Besides the scanned code, there are also some more or less useful attributes related to the scan.

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

On the Peakboard side, we set the MQTT broker and configure two subscriptions. Both listen at the topic `Peakboard/Gateway/PGGW402650394/scan`. This topic is constructed like this:
```
configured main topic + "Gateway" + serial number of the Gateway + the "scan" event
```

The Peakboard data source gives us the option to process the json right away by using so called data paths. We know from the sample JSON the scanned code can be found within the JSON under "scan_code", while the serial number is at "device_serial". And so we configure the two subscriptions that the JSON is translated into the scalar values we want to process later.

![image](/assets/2024-08-14/060.png)

The last thing we do is to preapre some text boxes to show the scanned code through data binding:

![image](/assets/2024-08-14/070.png)

If we want react directly to a scan we ideally put out script or Building Blocks into he refreshed event of this data source. It's triggered everytime a scan is coming in on the MQTT topic.

## Conclusion and result

Building a Peakboard app together with ProGlove scanners is straight forward. No matter if you use USB or MQTT mode. The video show the simple app and displays the scanned code in the bound text boxes.

{% include youtube.html id="tMobjoShVS0" %}
