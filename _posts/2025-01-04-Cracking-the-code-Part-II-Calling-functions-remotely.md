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
In our [first article about the Peakboard Hub API](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html) we learned how to to get an API key, establish the connection to the Hub and get information about the boxes that are connected to the hub.
In today's article we will discuss, how to call shared, public functions on a box from the outside. So let's assume you're running a third paty application and you need to inform people within the factory about a certain things and submit information to them. In that use case we would call the Peakboard Hub API to let the Hub initiated a function on the Peakboard application that is running on one or more boxes. In the example of this article we will submit an alarm to the box along with a paramter that states how any seconds the alarm is supposed to be shown. A second function is there to check, if currently an alarm is displayed. So we can use it to confirm that the alarm is shown.

## Build the Peakboatd application

The Peakboard application is very simple. In the center of the screen we have one single text box showing the alarm in red color ("N/A" if no alarm is set). We use the integer variable "SecondsLeft" to count the number of seconds that are left to display the alarm.

![image](/assets/2025-01-04/010.png)

Let's have a look at the first function "SubmitAlarm". It takes two paramaters: "AlarmTime" for the number of seconds to show the alarm message. And "AlarmMessage", a string to represent the alarm message itself. When the function is called, the text box shows the alarm message and the number of seconds is stored in the variable.

![image](/assets/2025-01-04/020.png)

The second function "IsAlarmActive" doesn't take any parameters, but returns a boolean variable depending if the the number of seconds is greater than 0 (alarm is active) or not (no alarm is active).

![image](/assets/2025-01-04/030.png)

The function within the timer event is just for counting down the seconds which are left and then setting the text back to "N/A" after the time is up.

![image](/assets/2025-01-04/040.png)

## Calling the API

Let's assume the application is actively running on a box and the box is connected to the Hub. The screenshot show the box in the Hub portal.

![image](/assets/2025-01-04/040.png)

The first we need to do is to connect to the Hub and exchange the API key with a access token we will need for the real calls later.

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

As some kind of additional exercise, we will now query some information with "/public-api/v1/box/functions" about what functions are available on our box. The response baody will also contain all informations about the function and parameters. In this case we just list the names of the available functions.

{% highlight python %}

# Get metadata of the shared functions of a box

response = mySession.get(BaseURL + "/public-api/v1/box/functions?boxId=PB0000PT")

if response.status_code != 200:
    sys.exit("Unable to obtain the functions of a box")

print(response.json())

for item in response.json():
    print(f"Function found: {item['Name']}")
{% endhighlight %}

the output of this code in our example will be:

![image](/assets/2025-01-04/060.png)

Here's the whole JSON response with all the metadata. We can see the two function with its corresponding in and out parameters:

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

Let's call the shared function now by using "/public-api/v1/box/function". We can see in the body how a dedicated box is addressed with its name. We also submit the name of the function to be called along with the parameters.

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

The second function we want to call is the "IsAlarmActive" function. It has a return parameter, that can be read in the response body. It's not emebedded in any kind of JSON, but printed directly in the body.

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

Here's the result in console output:

![image](/assets/2025-01-04/070.png)
