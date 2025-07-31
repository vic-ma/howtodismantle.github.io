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

First, we'll explain how the seven API works. Then, we'll build a small Peakboard application that sends out an SMS message to a phone number. Seven also offers an [email-to-SMS](https://www.seven.io/en/products/email-to-sms/) service for those that don't want to use the API.

## The seven.io portal

Creating an account in the seven.io portal is free of charge and come with a free plan for some indivual tests of the API without the need to pay any money. Within the portal we can manage the payment plans and also the configuration for those who need to build processing of incoming sms.

For our example we need to create an API key that covers at least the scope "SMS".

![image](/assets/2025-07-31/010.png)

## The UI

For our sample we chosse a simple UI with two text fields and a button.

![image](/assets/2025-07-31/020.png)

## The API

The API documentation for seven.io services can be found [here](https://docs.seven.io/en). The call we use in our example is epxlained [here](https://docs.seven.io/en/rest-api/endpoints/sms#send-sms).

So the actual payload is submitted through query parameters of a HTPP GET call that follows this pattern:

{% highlight url %}
https://gateway.seven.io/api/sms?to=#[to]#&from=#[from]#&text=#[text]#
{% endhighlight %}

- "to" is the number destintion number
- "from" is the sender number or just the sender's name
- "text" is the actual text message

Beside this we need to submit the API key in a header named "X-Api-Key". In the Peakboard Building block we just use a text placeholder block to build the correct URL including the query parameters. That's all.

![image](/assets/2025-07-31/030.png)

Currently we're not evaluating the response form the API but just writing it into the log. The API returns a code to that lets us precisely determine success or error reasons. The codes can be check [here](https://docs.seven.io/en/rest-api/endpoints/sms#return-codes).

## result

The screenshot shows the incoming SMS. 
As some kind of expansion to our very quick and easy example here are some thoughts for other cool stuff you can do with the seven.io portal.

- Of course seven.io API works too in all Hub Flows. It might even more appropriate to process the seven.io through a central Hub Flow rather than a single Peakboard application
- We can use seven.io to receive SMS and process them in a Peakboard application or a Hub Flow.
- seven.io also supports sending out [voice calls](https://docs.seven.io/en/rest-api/endpoints/voice#send-voice-call). Imagine a machine calling the supervisor and tell him  details about his current error state. How cool is that? The voice message is as easy to build as sending an SMS.

![image](/assets/2025-07-31/040.jpeg)
