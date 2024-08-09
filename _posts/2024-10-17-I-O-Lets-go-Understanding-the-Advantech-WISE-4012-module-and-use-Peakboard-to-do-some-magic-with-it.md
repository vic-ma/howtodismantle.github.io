---
layout: post
title: I/O, Let's go - Understanding the Advantech WISE-4012 module and use Peakboard to do some magic with it
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt api
image: /assets/2024-10-17/title.png
read_more_links:
  - name: Wise 4012 operating manual
    url: https://www.digimax.it/media_import/INDUSTRIAL%20PC/ADVANTECH/IoT%20-%20INTERNET%20OF%20THINGS/WISE-4012/WISE-4012_MAN_001.pdf
  - name: Unleashing the ICP DAS ET-2254 with MQTT and Peakboard
    url: /I-O-Lets-Go-Unleashing-the-ICP-DAS-ET-2254-with-MQTT-and-Peakboard.html
downloads:
  - name: Wise4012TestBoard.pbmx
    url: /assets/2024-10-17/Wise4012TestBoard.pbmx
---
In the past few weeks, we've discussed two I/O modules provided by ICP DAS: the [ET-2254](/I-O-Lets-Go-Unleashing-the-ICP-DAS-ET-2254-with-MQTT-and-Peakboard.html) and the [U-7560M](/I-O-Lets-Go-Unleashing-the-ICP-DAS-U-7500-series-for-cool-I-O-action-with-OPC-UA.html). In this article, we'll take a look at the Advantech WISE-4012. This module provides a network connection via Wi-Fi. It offers 2 digital-only outputs and 4 digital/analog inputs.

We will look at the traditional way of communicating with MQTT, as well as an alternative method: The REST webservice. Depending on your use case, REST may be better than MQTT, because no MQTT broker is necessary. 

![image](/assets/2024-10-17/010.png)

## Module Configuration

The Wise-4012 comes with a typical interface to configure the module. Unter the 'cloud' tab we find the option to activate MQTT connectivity and provide a broker for sending and receiving messages. The actual message consists of only one JSON string that covers all inputs and outputs.

![image](/assets/2024-10-17/020.png)

Under 'I/O Status'  we can define whether the 4 inputs are used as analog or digital. In our case we use channel 1 for analog input.

![image](/assets/2024-10-17/030.png)

To check if there's already a real value on the analog input the next tab 'AI' provides insight of the current that is measured at the input.

![image](/assets/2024-10-17/040.png)

## Mastering MQTT access

The next screenshot shows how to configure the MQTT datasource on the Peakboard side. The incoming message is a simple JSON file and we're using data path to access the information within the JSON. As shown in the image the actual values are translated directly to the columns of the output table, so no need to really process the JSON. It's done by the data source automatically.

{% highlight json %}
{
  "s": 1,
  "t": "2024-08-08T17:48:21Z",
  "q": 192,
  "c": 0,
  "di1": true,
  "di3": false,
  "di4": false,
  "do1": true,
  "do2": false,
  "ai2": 7158.464,
  "ai_st2": 1
}
{% endhighlight %}

![image](/assets/2024-10-17/050.png)

Let's discuss the other direction. In out test board (that can be download [here](/assets/2024-10-17/Wise4012TestBoard.pbmx)) there are two buttons for switching the output on or off.

![image](/assets/2024-10-17/060.png)

Let's have a look how to send the MQTT message. We're re-using the MQTT connection from the data source. The topic is 'Advantech/74FE488E8B86/ctl/do1', which is the serial number, fixed 'ctl' and then followed by the output channel name. The JSON to be sent is pretty simple: '{"v":true}' for swithcing on and '{"v":false}' for switching off.

![image](/assets/2024-10-17/070.png)

## Webservice and JSON

Beside the traditional MQTT connecivity the Wise-4012 also offers REST endpoints to check the state of the input channels and set the output channels. 

For the digital input we can use the http://<MyServer>/di_value/slot_0 endpoint. The screenshot shows the JSON data source and the structure that is returned and contains all inputs. So we use the path 'DIVal[0]' to access the first channel and get the value.

![image](/assets/2024-10-17/080.png)

For the analog input the endpoints is a bit different: http://<MyServer>/ai_value/slot_0 and so is the JSON that contains the analog input data.

![image](/assets/2024-10-17/090.png)

For setting the value of an output we use the endpoint http://<MyServer>/do_value/slot_0 and submit the JSON string '{"DOVal":[{"Ch":0,"Val":1}]}' which is quite similiar to the MQTT message we used earlier. It's very important to use the verb PATCH to submit the message.A regular PUT won't do the trick. Here's the Building Block to do the call:


![image](/assets/2024-10-17/100.png)

## result and conclusion

Having both options MQTT and REST is a great feature of the Wise-4012. However the video also shows the downside. Since MQTT is an event based prototcol any change in value is transferred and processed immediately. Getting input values via REST is always a pull request done every couple of seconds. So when it comes to very precise real time input, MQTT is a better choice. If the real time is not that important and a lag up a couple of seconds is acceptabel, REST might be easier because no MQTT broker is needed.

![image](/assets/2024-10-17/result.gif)
