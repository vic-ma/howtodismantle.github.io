---
layout: post
title: Ding! Your Machine Just Texted You – Powered by Peakboard + seven.io
date: 2023-03-01 00:00:00 +0000
tags: api
image: /assets/2025-07-31/title.png
image_header: /assets/2025-07-31/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Seven API documentation
    url: https://docs.seven.io/en
  - name: Seven SMS endpoint legacy methods
    url: https://docs.seven.io/en/rest-api/endpoints/sms#legacy
  - name: Seven SMS endpoint return codes
    url: https://docs.seven.io/en/rest-api/endpoints/sms#return-codes
downloads:
  - name: SevenIO_SMS.pbmx
    url: /assets/2025-07-31/SevenIO_SMS.pbmx
---
One the most important jobs of a Peakboard application is to communicate with the outside world. In today's article, we'll take a look at the [seven API](https://www.seven.io/en/products/sms-gateway-api/), a business messaging gateway service that makes it easy for businesses to send and receive SMS.

First, we'll explain how the seven API works. Then, we'll build a small Peakboard application that sends an SMS message to a phone number. Seven also offers an [email-to-SMS service](https://www.seven.io/en/products/email-to-sms/) if you don't want to use the API.

## The seven portal

You can create a free account in the seven portal. This lets you test out the API  without paying any money. Inside the portal, you can also enter a payment method, in order to use the API in production. You can also customize how incoming SMS messages are processed.

Now, lets build our application.

## API key

First, we create a seven API key that covers the `SMS` scope:

![image](/assets/2025-07-31/010.png)

## The UI

For our Peakboard app, we create a simple UI with two text fields and a button:

![image](/assets/2025-07-31/020.png)

## The API

We use the [legacy method of seven's SMS endpoint](https://docs.seven.io/en/rest-api/endpoints/sms#legacy).

We send a `GET` request to the `/api/sms/` endpoint, and we use query parameters to specify the SMS message we want to send:

{% highlight url %}
https://gateway.seven.io/api/sms?to=#[to]#&from=#[from]#&text=#[text]#
{% endhighlight %}

| Query parameter | Description |
| --------------- | ----------- |
| `to`            | The phone number that receives the SMS message.
| `from`          | The sender's phone number, or the sender's name.
| `text`          | The contents of the SMS message.

We also submit the API key in a header called `X-Api-Key`, in order to authenticate ourselves.

In Building Blocks, we use use a text placeholder block to build the correct URL, with all the query parameters:

![image](/assets/2025-07-31/030.png)

For this example application, we don't evaluate the response from the API. All we do is write the response into the application log. The API returns a code that lets us know if the call was a success or failure. For more information, see the [SMS endpoint return codes](https://docs.seven.io/en/rest-api/endpoints/sms#return-codes).

To learn more about the seven API in general, see the [seven API documentation](https://docs.seven.io/en).

## Result

This was a very basic example. There is much more cool stuff you can do with seven. Here are some ideas:
- The seven API works in all Hub Flows. So, it might make sense to handle seven API calls in a central Hub Flow, rather than an individual Peakboard application.
- You can use the seven API to receive SMS messages and process them in a Peakboard application or a Hub Flow.
- The seven API can also send [voice calls](https://docs.seven.io/en/rest-api/endpoints/voice#send-voice-call). Imagine a machine calling the supervisor to explain the details of an issue. How cool is that! The voice message API is as easy to use as the SMS API we looked at today.

This screenshot shows the incoming SMS sent by our application, via seven:

![image](/assets/2025-07-31/040.jpeg)
