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
With Peakboard Hub Online, it's easy to connect cloud apps and services to applications that run in a worker's workplace in a factory. That's one of the main benefits of connecting Peakboard Boxes to the Hub. In this article we'll explain how to use an Azure Logic App to build a simple workflow that calls functions in a Peakboard app. The possibilities of this technique are endless.

The Azure Logic App uses the standard API to communicate with the Hub. And the Hub has a secure connection through the firewall of the customer's factory, to the Box. This makes it perfectly secure to bridge the gap between cloud services and apps and any kind of entity that sits directly in the highly sensitive area of production IT.

## The Peakboard app and other requirements

In our example we use the Alarm app that we already discussed in the [article about how to call a function by using the API](/Cracking-the-code-Part-II-Calling-functions-remotely.html). This app exposes a function called "SubmitAlarm" and just displays the alarm message in the screen for a given number of seconds (which is the second parameter for the function).

We want to use the Azure Logic App later to trigger that function on the box. The logic of the function is very simple.

![image](/assets/2025-02-05/020.png)

The box we want to use is registered in the Peakboard Hub.

![image](/assets/2025-02-05/030.png)

And also we need an API key with at least the scope "write:box" to allow a 3rd party caller to execute the function.

![image](/assets/2025-02-05/040.png)

## Build the Azure Logic App

Before we steps into the details of each step, how does our Logic work? We need three steps to call the function:

1. Authenticate against the Hub API with our API key
2. Parse the JSON string that is returned by the authentification
3. Call the actualy API to execute the function

After having a created a new Logic App in the Azure portal, we add the first http call as shown in the screenshot. The endpoint GET "/auth/token" receives the API key within an additional header with key "apiKey".

![image](/assets/2025-02-05/050.png)

The actual access token is returned in the JSON string of the response body. To make the token available we use a "Parse JSON" block in the logic app. The schema that is used for parsing the JSON string can either by typed manually or just copied from this template.

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

For the actual alarm to be submitted we call the endpoint POST "/box/function". The body of the request contains the JSON to trigger the right function on the right box with the right parameters:

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

Here's how the call looks like in the design mode. 

![image](/assets/2025-02-05/070.png)

The additional header "Authorization" is a simple concatenate from the term "Bearer " with the token that is extracted from the JSON in the previous parsing step.

![image](/assets/2025-02-05/080.png)

Of course in our sample we don't catch any kind of exceptions or other problems that might occur to make sure the example is easy to understand. In a real world application we must consider that authentification might go wrong or the box is not available to be called.

## result

We can try out the app simply by running the Logic App manually. ALl workflow steps should have green check marks if no problems occur.

![image](/assets/2025-02-05/090.png)

And the alarm is submitted to the box and displayed on the screen.

![image](/assets/2025-02-05/100.png)
