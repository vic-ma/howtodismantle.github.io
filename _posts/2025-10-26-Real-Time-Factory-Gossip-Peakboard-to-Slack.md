---
layout: post
title: Real-Time Factory Gossip - Peakboard to Slack
date: 2023-03-01 05:00:00 +0000
tags: api
image: /assets/2025-10-26/title.png
image_header: /assets/2025-10-26/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Slack Apps
    url: https://api.slack.com/apps
  - name: How to use incoming webhooks
    url: https://docs.slack.dev/messaging/sending-messages-using-incoming-webhooks
downloads:
  - name: SendSlackMessage.pbmx
    url: /assets/2025-10-26/SendSlackMessage.pbmx
---
A lot of Peakboard applications circle around communication with the outside world. In today's article, we will discuss to send Slack messages from your Peakboard application. A typical example would be to be connected to a machine, for example, by OPC UA, and then process this information, and then  generate alerts and send it to Slack. 

So what we're going to do today is first we set up an app in the Slack settings and then in the second step we will use a Peakboard application to trigger an incoming webhook to send a message to a channel. Let's assume the Peakboard application is talking to a welding machine and sends certain alerts to Slack.

## Setting up the Slack App

Before we can post messages to a Slack channel, we need to set up a Slack app first. Setting up a Slack app can be done under this [link](https://api.slack.com/apps). 

First, we create a new app from scratch. 

![image](/assets/2025-10-26/010.png)

Then we give it a name and connect it to our Slack workspace. Besides the name, it's always a good idea to also upload an icon to make it more beatiful. 

![image](/assets/2025-10-26/020.png)

On the left side, we click on incoming webhooks and enable these incoming webhooks. Then we have the option to create a new webhook and copy the generated URL to the clipboard. 

![image](/assets/2025-10-26/030.png)

## Building the Peakboard app

In our application, we place a text box and a button on the screen, and it's important to give the text box a name so we can use it later in our script. 

![image](/assets/2025-10-26/040.png)

The following screenshot shows how to call the webhook with the Building Blocks. We just use an HTTP Call Building Block. We use HTTP POST call method. The actual body is relatively simple because we only transport our message: `{ "text": "My Message" }`. The message is replaced with the actual content of the user message by using a placeholder building block. 

![image](/assets/2025-10-26/050.png)

## result

The next two screenshots show the result in action. First, we type in the message we want to send and then send the message. 

![image](/assets/2025-10-26/060.png)

The incoming webhook is triggered and that generates the message in the Slack channel that we have defined when creating the app earlier in the Slack settings. 

![image](/assets/2025-10-26/070.png)

Under this [link](https://docs.slack.dev/messaging/sending-messages-using-incoming-webhooks) we can find the documentation of the procedure we just used. It's  possible to use lot of other formatting options to enrich the messages we send to Slack. 