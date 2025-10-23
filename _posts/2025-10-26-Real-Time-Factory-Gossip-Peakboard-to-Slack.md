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
Many Peakboard applications are designed to communicate with the outside world. In today's article, we'll explain how to use a Peakboard app to send Slack messages.

A common use case for this is an app that forwards messages from a machine to a Slack channel. For example:
1. A machine sends a message to the Peakboard app, via OPC UA.
1. The Peakboard app processes the message.
1. The Peakboard app sends the message to a Slack channel dedicated for machine alerts.
1. A human sees the message on Slack and acts accordingly.

In this article, we'll build a simpler app that let's the user write a message and then send it to a Slack channel. That way, we don't have to worry about connecting the app to an actual machine, since our goal is just to show how the Slack integration works.  But we have plenty of other articles on [hardware integration](/category/hardware), if you're interested.

## Create a Slack App

In order for our Peakboard app to send messages to a Slack channel, we first need to [create a Slack app](https://api.slack.com/apps). 

We select the *From scratch* option.

![image](/assets/2025-10-26/010.png)

We give it a name and connect it to our Slack workspace. (It's also a good idea to upload an icon to make it more pretty.) Then, we click *Create App*. 

![image](/assets/2025-10-26/020.png)

Next, on the sidebar, under *Features*, we click on *Incoming Webhooks*. We turn on the *Activate Incoming Webhooks* toggle switch.

Then, we create a new webhook and copy the URL. We will need it when building our Peakboard app.

![image](/assets/2025-10-26/030.png)

## Build the Peakboard app

Now, let's build our Peakboard app. First, we create the UI:
1. We add a text box where the user can enter a message. We give it the control name `txtMessageText`. That way, we can refer to it later on, in our script.
1. We add a button that the user can press to send the message to Slack.

![image](/assets/2025-10-26/040.png)

Next, we create the tapped script for our button:

![image](/assets/2025-10-26/050.png)

It sends a POST request to the Slack webhook URL that we copied earlier. We set the body of the request to `{ "text": "#[Message]#" }`. We use the `#[Message]#` placeholder, and replace it with the message that the user entered in the text box. Finally, we also write the response that we get (after sending the request) to the log.

## result

The next two screenshots show the result in action. First, we type in the message we want to send and then send the message. 

![image](/assets/2025-10-26/060.png)

The incoming webhook is triggered and that generates the message in the Slack channel that we have defined when creating the app earlier in the Slack settings. 

![image](/assets/2025-10-26/070.png)

Under this [link](https://docs.slack.dev/messaging/sending-messages-using-incoming-webhooks) we can find the documentation of the procedure we just used. It's  possible to use lot of other formatting options to enrich the messages we send to Slack. 