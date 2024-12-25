---
layout: post
title: Cracking the code - Part II - Calling functions remotely by using Peakboard Hub API
date: 2023-03-01 12:00:00 +0200
tags: api peakboardhub peakboardhubapi
image: /assets/2025-01-04/title.png
image_header: /assets/2025-01-04/title_landscape.png
read_more_links:
  - name: Cracking the code - Part I - Getting started with Peakboard Hub API
    url: /Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html
  - name: Cracking the code - Part III - Reading and writing lists with Peakboard Hub API
    url: /Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html
downloads:
  - name: PeakboardHubAPIFunctions.py
    url: /assets/2025-01-04/PeakboardHubAPIFunctions.py
---
In our [first article](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html) about the Peakboard Hub API, we discussed how to get an API key, establish a connection to the Hub, and get information about the boxes that are connected to the hub.
In today's article, we will discuss how to call shared, public functions on a box from the outside.

Let's assume we're running a third party application, and we need to notify people inside our factory about something. In this case, we can call the Peakboard Hub API to have the Hub run a function on a Peakboard Box.

In this article, we will run an alarm on the box and send a parameter that specifies how many seconds the alarm should run for. We will also use a second function to check if an alarm is currently displayed. That way, we can use it to confirm that the alarm is working properly.

## Build the Peakboard application

The Peakboard application is simple. In the center of the screen, we have a text box:
* If there's an alarm, it shows the alarm message in a red color.
* If there's no alarm, it shows "N/A."

We use the integer variable `SecondsLeft` to track the number of seconds the alarm will continue running for.

![image](/assets/2025-01-04/010.png)

Let's take a look at the first function, `SubmitAlarm`. It takes two parameters:
* `AlarmTime`, the number of seconds to display the alarm message.
* `AlarmMessage`, a string that contains the alarm message itself.

When `SubmitAlarm` is called, the text box shows the alarm message, and the number of seconds is stored in the variable.

![image](/assets/2025-01-04/020.png)

The second function, `IsAlarmActive`, doesn't take any parameters. It returns a boolean that specifies if the number of seconds remaining on the alarm is greater than 0 (alarm is active) or not (alarm is inactive).

![image](/assets/2025-01-04/030.png)

The function in the timer event is used to count the time that's left, and to set the text back to "N/A" once the time is up.

![image](/assets/2025-01-04/040.png)

## Call the API

Let's assume the application is actively running on a box and the box is connected to the Hub. This screenshot shows the box in the Hub portal:

![image](/assets/2025-01-04/040.png)

The first thing we need to do is connect to the Hub and exchange the API key with an access token that we will need for the real calls later.

{% highlight python %}
import requests
import sys
import pandas

BaseURL = "https://api.peakboard.com";
APIKey = "XXX";

response = requests.get(BaseURL + "/public-api/v1/auth/token", headers={'apiKey': APIKey})

if response.status_code != 200:
    sys.exit("Unable to obtain a access token, so quitting") 

accesstoken = response.json()["accessToken"]
print("Succesfully authorized until " + response.json()["validUntill"])

mySession = requests.Session()
mySession.headers.update({"Authorization": "Bearer " + accesstoken})
{% endhighlight %}

As an additional exercise, we will get the functions that are available on our box with the `/public-api/v1/box/functions` endpoint. The response body contains comprehensive information about the functions and their parameters. In our case, we only need the names of the available functions.

{% highlight python %}

# Get metadata of the shared functions of a box

response = mySession.get(BaseURL + "/public-api/v1/box/functions?boxId=PB0000PT")

if response.status_code != 200:
    sys.exit("Unable to obtain the functions of a box")

print(response.json())

for item in response.json():
    print(f"Function found: {item['Name']}")
{% endhighlight %}

The output of this code looks like:

![image](/assets/2025-01-04/060.png)

Here's the entire JSON response with all the metadata. We can see the two functions with their corresponding in and out parameters:

{% highlight python %}
[ {
      "Name":"SubmitAlarm",
      "Description":"None",
      "ReturnType":"None",
      "Parameters":[
         {
            "ParameterName":"AlarmTime",
            "ParameterType":"Number",
            "Description":"None"
         },
         {
            "ParameterName":"AlarmMessage",
            "ParameterType":"String",
            "Description":"None"
         }
      ]
   },
   {
      "Name":"IsAlarmActive",
      "Description":"None",
      "ReturnType":"Boolean",
      "Parameters":[
         
      ]
   }]
{% endhighlight %}

# Call a function

We call the shared function with the `/public-api/v1/box/function` endpoint. You can see in the body how a dedicated box is addressed with its name. We also submit the name of the function to be called along with the parameters.

{% highlight python %}
body = {
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

response = mySession.post(BaseURL + "/public-api/v1/box/function", json=body)

if response.status_code != 200:
    sys.exit("Unable to call the function. Return is " + response.reason)

print("Alarm set succesfully....")
{% endhighlight %}

The second function we need to call is `IsAlarmActive`. It has a return parameter that can be read in the response body. It's not embedded in any kind of JSON, but rather printed directly in the body.

{% highlight python %}
## Call a function with return value

body = {
  "boxId": "PB0000PT",
  "functionName": "IsAlarmActive"
}

response = mySession.post(BaseURL + "/public-api/v1/box/function", json=body)

if response.status_code != 200:
    sys.exit("Unable to call the function. Return is " + response.reason)

print(f"Is alarm set? -> {response.text}")
{% endhighlight %}

Here's the result in the console output:

![image](/assets/2025-01-04/070.png)

And here's the actual alarm output on the box, as seen from the Peakboard Hub portal:

![image](/assets/2025-01-04/080.png)

