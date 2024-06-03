---
layout: post
title: Barcode Bliss - Part II - Sending Feedback to ProGlove Scanners
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt
image: /assets/2024-08-30/title.png
read_more_links:
  - name: Barcode Bliss - Part I - Integrating ProGlove Scanners with Peakboard
    url: /Barcode-Bliss-Part-I-Integrating-ProGlove-Scanners-with-Peakboard.html
  - name: ProGlove Documentation
    url: https://docs.proglove.com/en/connect-gateway-to-your-network-using-mqtt-integration.html
downloads:
  - name: ProGloveTestCenter.pbmx
    url: /assets/2024-08-14/ProGloveTestCenter.pbmx
---
In the [first part](/2024-08-14-Barcode-Bliss-Part-I-Integrating-ProGlove-Scanners-with-Peakboard.html) of our mini series we already got familiar with the some basic information on how to integrate ProGlove scanners in Peakboard applications. We used both USB and MQTT to get the scan event along with some metadata.

In this article we will discuss some options, on how to give the scanner user feedback about the current scan. This is especially interesting for the implementation of processes where the scanner user doesn't want to look at the screen all the time. But in case something goes wrong might get back to the screen to find out more details. A typical use case for this pattern would be to check all products of a order. The scanner user scans all products. If there are products which doesn't fit into the order, he gets a negative feedback. 

We will discuss two examples. One with just positive / negative feedback and one with more even more information: A storage bin and some attributes of the scanned product will show up on the scanner display.

## Feedback by light

The [Mark 3 Model](https://proglove.com/products/hardware/mark-3/) is equipped with the LEDs in variable colors to give feedback. Giving feedback to the scanner via MQTT works in the same way with submitting a scan event, only the direction and the JSON changes.

The topic we're sending our message to is "Peakboard/gateway/PGGW402650394/feedback!". "Peakboard" is the configured main topic, "gateway" is just keyword because we're adressing the gateway, "PGGW402650394" is the serial number of the gateway, "feedback!" is the event type we're triggering.

The details can be checked at the [ProGlove Docs](https://docs.proglove.com/en/worker-feedback-command.html).

Here's the JSON string to be send giving feedback to the scanner:

{% highlight json %}
{
  "api_version": "1.0",
  "event_type": "feedback!",
  "event_id": "02114da8-feae-46e3-8b00-a3f7ea8672df",
  "time_created": 1546300800000,
  "device_serial": "M2MR111100928",
  "feedback_action_id": "FEEDBACK_POSITIVE"
}
{% endhighlight %}

There are two important attributes within the JSON: "device_serial" must be set to the serial number of the scanner we want the feedback send to. This is superimportant because there could be more than one scanner connected to the gateway. The "feedback_action_id" is a constant defining which light to be flashed. In our sample we use FEEDBACK_POSITIVE for the green light and FEEDBACK_NEGATIVE for the red light.

In our demo environment we place two buttons to showcase the feedback function.

![image](/assets/2024-08-30/010.png)

The actual MQTT message is sent with one single MQTT Publish command. We see in the screenshot, that we just send the JSON string to the topic discussed above by using the existing MQTT connection which refers to the initial data source we used in the other article.
Then we use a multiline string with placeholders to exchange the placeholder #[SerialNo]# with the serial number from the last scan. We just look this up from the first row in the data source.
The second button works excatly the same but uses FEEDBACK_NEGATIVE within the JSON instead.

![image](/assets/2024-08-30/020.png)

The video shows how the scan of the canned tomatoes is presented on the screen. And then a positive and a negative feedback is sent back to the scanner to light up the LEDs. 

{% include youtube.html id="EFzW1Y6QYvA" %}

## Feedback by Display

In this paragraph we try out another ProGlove model, the [Mark Display](https://proglove.com/products/hardware/mark-display/).
It offers even more options to give the scanneruser a feedback because it comes with a Display.

From a technical standpoint, setting the display content works similiar to operate the LEDs. We will just send a "display!" MQTT message to the gateway.
ProGlove offers different kind of templates for displaying the message on the display. These templates can be seen in the [documentation](https://docs.proglove.com/en/screen-templates.html). In our case we use a simple one called PG1 with two variable fields, a header and a body text.

The following JSON string shows a sample of the "display!" command. Beside the name of the template and the serial number of the destintion scanner, there are two variable fields we need to fill: "display_field_header" is the upper part of the display template, "display_field_text" is the lower part with small font.

{% highlight json %}
{
    "api_version": "1.0",
    "event_type": "display!",
    "event_id": "21d22d49-efbe-4e0c-b109-2e9afb7acacc",
    "time_created": 1546300800000,
    "time_validity_duration": 0,
    "device_serial": "MDMR000006406",
    "display_template_id": "PG1",
    "display_refresh_type": "DEFAULT",
    "display_fields": [{
        "display_field_id": 1,
        "display_field_header": "Storage Unit",
        "display_field_text": "R15"
    }]
}
{% endhighlight %}

Let's switch to the Peakboard app. In our test app we use two dynamic text fields to define the values and a button to iniate the process.

![image](/assets/2024-08-30/030.png)

What happens behind the button is quite similiar to our first example. Instaad of only one we now have three placeholder to dynamically create the JSON string with the serial number and two dynamic display texts.

![image](/assets/2024-08-30/040.png)

In the video we can see the final result. A barcode is scanned an then the feedback is sent back to to scanner to manipulate the display.

{% include youtube.html id="dfIobBdu6-w" %}
