---
layout: post
title: Cloud to Factory - Building an Azure Logic App to Access Peakboard Boxes with Peakboard Hub
date: 2023-03-01 00:00:00 +0200
tags: peakboardhubapi peakboardhub
image: /assets/2025-02-05/title.png
image_header: /assets/2025-02-05/title_landscape.png
read_more_links:
  - name: Cracking the code - Part I - Getting started with Peakboard Hub API
    url: /Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html
downloads:
  - name: AlarmScreen.pbmx
    url: /assets/2025-01-04/AlarmScreen.pbmx 
---
With Peakboard Hub Online, it's easy to connect cloud apps and services to applications that run in a worker's workplace in a factory. That's one of the main benefits of connecting Peakboard Boxes to the Hub. In this article, we'll explain how to use an Azure logic app to build a simple workflow that calls functions in a Peakboard app. The possibilities of this technique are endless.

The Azure logic app uses the standard Hub API to communicate with the Hub. And the Hub has a secure connection through the firewall of the customer's factory, to the Box. This makes it perfectly secure to bridge the gap between cloud services/apps and any entity that sits in the highly secure area of production IT.

## The Peakboard app and other requirements

For our Peakboard app, we'll use the Alarm app that we built in our article about [calling functions with the Peakboard Hub API](/Cracking-the-code-Part-II-Calling-functions-remotely.html). The app exposes a function called `SubmitAlarm`. This function displays a message on screen for a number of seconds. The message and number of seconds are the parameters of the function

We want to use the logic app to trigger this function on the Box. The logic of the function is simple:

![image](/assets/2025-02-05/020.png)

The Box we want to use is registered in Peakboard Hub:

![image](/assets/2025-02-05/030.png)

We also need an API key with a minimum scope of `write:box`. This will allow a third-party caller (like the logic app) to execute the Box function.

![image](/assets/2025-02-05/040.png)

## Build the logic app

Here's what the logic app needs to do to call the Box function:

1. Authenticate against the Hub API with our API key.
2. Parse the JSON string that is returned.
3. Call the Hub API to execute the Box function.

We create a new logic app in the Azure portal. Then, we add the first HTTP call, as shown in the following screenshot. The endpoint `GET /auth/token` accepts the API key in a header called `apiKey`.

![image](/assets/2025-02-05/050.png)

The access token is returned in the JSON string response body. To make the token available, we use a "Parse JSON" block in the logic app. Here's the schema for parsing the JSON string:

{% highlight json %}
{
    "type": "object",
    "properties": {
        "accessToken": {
            "type": "string"
        }
    }
}
{% endhighlight %}

![image](/assets/2025-02-05/060.png)

To submit the alarm request, we call the endpoint `POST /box/function`. The body of the request is a JSON string that specifies the Box, function, and parameters:

{% highlight json %}
{
  "boxId": "PB0000PT",
  "functionName": "SubmitAlarm",
  "parameters": [
    {
      "name": "AlarmTime",
      "value": 10
    },
    {
      "name": "AlarmMessage",
      "value": "We have a serious problem here"
    }
  ]
}
{% endhighlight %}

Here's what the call looks like in the design mode. 

![image](/assets/2025-02-05/070.png)

The additional header, `Authorization`, is a simple concatenation of the term `Bearer ` and the token that is extracted from the JSON string in the previous parsing step.

![image](/assets/2025-02-05/080.png)

Of course, in our example, we don't catch any kind of exceptions or other problems that might occur. This is to make sure the example is easy to understand. In a real world application, we must consider that authentication could fail, or the box might not be available.

## Result

You can try out the app by running the logic app manually. All the workflow steps should have green checkmarks if there are no problems.

![image](/assets/2025-02-05/090.png)

The alarm request is submitted to the Box and displayed on screen:

![image](/assets/2025-02-05/100.png)
