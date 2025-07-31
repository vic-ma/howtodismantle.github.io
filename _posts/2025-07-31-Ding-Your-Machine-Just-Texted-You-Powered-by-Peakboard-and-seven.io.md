---
layout: post
title: Ding! Your Machine Just Texted You – Powered by Peakboard + seven.io
date: 2023-03-01 00:00:00 +0000
tags: api
image: /assets/2025-07-31/title.png
image_header: /assets/2025-07-31/title_landscape.png
bg_alternative: true
read_more_links:
  - name: seven API documentation
    url: https://docs.seven.io/en
  - name: seven SMS endpoint return codes
    url: https://docs.seven.io/en/rest-api/endpoints/sms#return-codes
downloads:
  - name: SevenIO_SMS.pbmx
    url: /assets/2025-07-31/SevenIO_SMS.pbmx
---
One the most important jobs of a Peakboard application is to communicate with the outside world. In today's article, we'll take a look at the [seven API](https://www.seven.io/en/products/sms-gateway-api/), a business messaging gateway service that makes it easy for businesses to send and receive SMS.

First, we'll explain how the seven API works. Then, we'll build a small Peakboard application that sends out an SMS message to a phone number. Seven also offers an [email-to-SMS service](https://www.seven.io/en/products/email-to-sms/) for those that don't want to use the API.

## The seven portal

You can create a free account in the seven portal. This lets you test out the API  without paying any money. Inside the portal, you can also select a payment plan. You can also change the configuration, in order to set up how incoming SMS messages are processed.

For our example, we create an API key that covers the `SMS` scope:

![image](/assets/2025-07-31/010.png)

## The UI

For our Peakboard app, we first create a simple UI with two text fields and a button:

![image](/assets/2025-07-31/020.png)

## The API

For our example, we use the [legacy method of seven's SMS endpoint](https://docs.seven.io/en/rest-api/endpoints/sms#legacy).

We send a `GET` request to the `api/sms/` endpoint, and we use query parameters to specify the SMS message we want to send:

{% highlight url %}
https://gateway.seven.io/api/sms?to=#[to]#&from=#[from]#&text=#[text]#
{% endhighlight %}

- "to" is the number destintion number
- "from" is the sender number or just the sender's name
- "text" is the actual text message

Beside this we need to submit the API key in a header named "X-Api-Key". In the Peakboard Building block we just use a text placeholder block to build the correct URL including the query parameters. That's all.

![image](/assets/2025-07-31/030.png)

Currently we're not evaluating the response form the API but just writing it into the log. The API returns a code to that lets us precisely determine success or error reasons. The codes can be check [here](https://docs.seven.io/en/rest-api/endpoints/sms#return-codes).

To learn about the seven API, you can check out the [seven API documentation](https://docs.seven.io/en).

## result

The screenshot shows the incoming SMS. 
As some kind of expansion to our very quick and easy example here are some thoughts for other cool stuff you can do with the seven.io portal.

- Of course seven.io API works too in all Hub Flows. It might even more appropriate to process the seven.io through a central Hub Flow rather than a single Peakboard application
- We can use seven.io to receive SMS and process them in a Peakboard application or a Hub Flow.
- seven.io also supports sending out [voice calls](https://docs.seven.io/en/rest-api/endpoints/voice#send-voice-call). Imagine a machine calling the supervisor and tell him  details about his current error state. How cool is that? The voice message is as easy to build as sending an SMS.

![image](/assets/2025-07-31/040.jpeg)
