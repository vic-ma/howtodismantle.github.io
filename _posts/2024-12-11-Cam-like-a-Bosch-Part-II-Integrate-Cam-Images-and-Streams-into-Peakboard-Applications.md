---
layout: post
title: Cam like a Bosch - Part II - Integrate Cam Images and Streams into Peakboard Applications
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2024-12-11/title.png
image_header: /assets/2024-12-11/title_landscape.jpg
read_more_links:
  - name: Cam like a Bosch - Part I - Integrate Bosch security cam AI features with Peakboard
    url: /Cam-like-a-Bosch-Part-I-Integrate-Bosch-security-cam-AI-features-with-Peakboard.html
  - name: Peakboard Hardware Guide
    url: /hardwareguide
  - name: Bosch Cam Product Page
    url: https://www.boschsecurity.com/us/en/news/product-news/flexidome-5100i/
  - name: YouTube - Integrate a Bosch IP camera system on your dashboard
    url: https://www.youtube.com/watch?v=ztthsCF4USw&ab_channel=PeakboardEN
  - name: Bosch Guide for using RTSP
    url: https://community.boschsecurity.com/varuj77995/attachments/varuj77995/bt_community-tkb-video/241/1/RTSP%20usage%20with%20Bosch%20Video%20IP%20Devices.pdf
  - name: How to request a RTSP multicast stream from a BOSCH IP camera?
    url: https://community.boschsecurity.com/t5/Security-Video/How-to-request-a-RTSP-multicast-stream-from-a-BOSCH-IP-camera/ta-p/16494
downloads:
  - name: BoschCamParkingLot.pbmx
    url: /assets/2024-11-09/BoschCamParkingLot.pbmx
---
A couple of weeks ago, we discussed how to use the Bosch security cam to [monitor the occupancy level of a parking lot](/Cam-like-a-Bosch-Part-I-Integrate-Bosch-security-cam-AI-features-with-Peakboard.html). In today's article, we'll discuss how to integrate the camera image into the Peakboard application.

There are two different methods:
* Regularly download and refresh a web image.
* Use the RTSP stream from the camera.

By embedding a web image rather than a stream, we reduce the technical complexity and network bandwidth. But the best choice is determined by the use case.

For monitoring a parking lot and its occupancy, the image option is the best choice, because the situation doesn't change within seconds. Instead, it should take minutes for the situation to change. So refreshing the image every minute is perfect sufficient for the viewer.

The story is different when we monitor moving traffic or even a machine with moving parts. Here, the use of streaming makes much more sense, because we'd want to inform the viewer immediately about any new information.

## Embed the camera picture

To embed the Bosch cam image, we use a regular web page control. In the URL field, we enter the URL of the image.

To authenticate against the source, we enter our credentials into the URL, just in front of the IP address or server name: `UserName:Password@ServerName/...`. Here's an example for our current demo environment:

{% highlight url %}
`http://service:Peakboard_2023@192.168.20.152/snap.jpg?JpegCam=1`
{% endhighlight %}

We can set the refresh time in seconds, as a control property. No need for any programming to manage the automatic refresh.

![image](/assets/2024-12-11/010.png)

## Configure the stream

Before we can use the camera stream in our application, we need to configure it carefully. It's important to set the stream resolution and the frame rate to a reasonable value. In our case, 512x288 pixels at 6 FPS (frames per second) is totally sufficient. The default values are often ridiculously high (Full HD or even 4k resolution with frame rate of 30 FPS). This unnecessarily wastes bandwidth, and could even cause the stream to fail, if the network can't handle the bandwidth.

The Bosch cam comes with 4 different streaming endpoints to support 4 different stream settings. The screenshot shows our configuration as the second stream.

![image](/assets/2024-12-11/020.png)

## Embed the stream in Peakboard

To embed the stream in the video control of our Peakboard application, we use a similar URL to the single image option, but with a different protocol. For streaming, we use [RTSP](https://en.wikipedia.org/wiki/Real-Time_Streaming_Protocol). Actually, all professional-grade cameras support this protocol.

The `inst` parameter defines the stream endpoint, as configured in the back end (in our case, it's stream endpoint 2), along with the username and password:

{% highlight url %}
rtsp://service:Peakboard_2023@192.168.20.152/?inst=2.
{% endhighlight %}

In case we want some more metadata to be displayed directly in the stream (e.g. the recognized types of vehicles), we can use the parameters `meta` and `vcd`. Then, the URL would look like this:

{% highlight url %}
rtsp://service:Peakboard_2023@192.168.20.152/?inst=2&meta=1&vcd=1
{% endhighlight %}

To find out more about the parameters and their usage, refer to the [PDF documentation](https://community.boschsecurity.com/varuj77995/attachments/varuj77995/bt_community-tkb-video/241/1/RTSP%20usage%20with%20Bosch%20Video%20IP%20Devices.pdf).

![image](/assets/2024-12-11/020.png)

## Result

Here's the result of our demo board for the parking lot. You can even see in the screenshot that the quality of the static image is better than the stream, because we configured the stream with a very low resolution.

![image](/assets/2024-12-11/result.png)
