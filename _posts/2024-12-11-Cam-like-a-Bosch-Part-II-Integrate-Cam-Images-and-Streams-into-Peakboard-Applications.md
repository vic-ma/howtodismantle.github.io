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
  - name: BoschCamParkingLotMonitoring.pbmx
    url: /assets/2024-11-09/BoschCamParkingLotMonitoring.pbmx
---
A couple of weeks ago we discussed how to use the Bosch security cam to [monitor the occupancy level of a parking lot](/Cam-like-a-Bosch-Part-I-Integrate-Bosch-security-cam-AI-features-with-Peakboard.html). In today's article we will discuss how to integrate the actual camera image in the Peakboard application. We can use two different basic technologies: Just a web image that is downloaded and refreshed reguarly, or we use the RTSP stream after we configured it in the camera's backend.

The big advantage of embedding a camera image rather than a stream is the technical complexity and also a massively reduced network bandwidth. But the best choice is defined by the actual use case. In our case, monitoring a parking lot and its occupancy, the image option is the samrtest choice because the situation doesn't change within seconds. It will most likely change within minutes. So refreshing the image on a minutely basis is perfecty sufficient for the viewer.
The story is completely different when we monitor moving traffic or even a machine with moving parts. Here the use of streaming makes much more sense to inform the viewer immedietely about any new situation.

## Embed the camera picture

For using the Bosch cam image we just use a regular Web Page control and just put in the URL for the image directly in the URL field.
As we need to authenticate against the source, we just put user name and passwort into the URL just in front of the IP or server name: "UserName:Password@ServerName/...". Here is the sample for our current demo envorment: "http://service:Peakboard_2023@192.168.20.152/snap.jpg?JpegCam=1"
The refresh time in seconds can be set as a control property. No need for any programming to manage the automatic refresh.

![image](/assets/2024-12-11/010.png)

## Configure the stream

Before we can use the camera stream in our application we need to configure it carefully. The screenshots show to web interface for the stream configuration. It is very important to adjust the stream resultion and the frame rate to a reasonable value. In our case 512x288 pixel with frame rate of 6 (frames per second) is totally sufficient. The default values are often redicolous high (Full HD or even 4k resolution with frame rate of 30 FPS). This does not only maximize the necessary bandwidth without any need. It can also lead to the problem, that the stream is not working at all, because the bandwidth is too high for the network.

The Bosch cam comes with 4 different streaming endpoints to support 4 different stream settings. The screenshot shows our configuration as the second stream.

![image](/assets/2024-12-11/020.png)

## Embedd the stream in Peakboard

For emebdding the stream in the Video Control of the Peakboard application, we use a similiar URL as the single image option, but a different protocol. For streaming we use [RTSP](https://en.wikipedia.org/wiki/Real-Time_Streaming_Protocol). Actually all cameras for professional use do support this protocol. The "inst" parameter defines the stream endpoint as confiugured in the back end (in our case it's stream endpoint 2), along with user name and password: "rtsp://service:Peakboard_2023@192.168.20.152/?inst=2".

In case we want some more metadata to be displayed directly in the stream (e.g. the recognized types of vehicles), we can use the parameters "meta" and "vcd". Then the URL would look like this: "rtsp://service:Peakboard_2023@192.168.20.152/?inst=2&meta=1&vcd=1".
To find out more about the parameters and their usage, we refer to the pdf documentation that is accessible [here](https://community.boschsecurity.com/varuj77995/attachments/varuj77995/bt_community-tkb-video/241/1/RTSP%20usage%20with%20Bosch%20Video%20IP%20Devices.pdf).

![image](/assets/2024-12-11/020.png)

## result

Here's the result of our demo board for the parking lot. We can even see in the screenshot that the actual qualtity of the static image is better than the stream as we have configured the stream in a very low resolution.

![image](/assets/2024-12-11/result.png)
