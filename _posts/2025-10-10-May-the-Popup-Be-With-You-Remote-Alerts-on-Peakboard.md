---
layout: post
title: May the Popup Be With You – Remote Alerts on Peakboard 
date: 2023-03-01 05:00:00 +0300
tags: api
image: /assets/2025-10-10/title.png
image_header: /assets/2025-10-10/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Cracking the code - Part II - Calling functions remotely by using Peakboard Hub API
    url: /Cracking-the-code-Part-II-Calling-functions-remotely.html
downloads:
  - name: PopupMessage.pbmx
    url: /assets/2025-10-10/PopupMessage.pbmx
---
In a previous article, we discussed how to use the Peakboard Hub API to [send a notification to a Peakboard Box](/Cracking-the-code-Part-II-Calling-functions-remotely.html). And it's especially cool when the Peakboard Box (or BYOD instance) is not directly reachable from the caller, or when the IP address is unknown. It works even with remote Boxes, so long as they are connected to Peakboard Hub Online.

But what if we are operating in a single facility, and we can access the Box directly? In that case, we don't need to use the Hub API at all---we can just send our notification directly to the Box. And in today's article, we'll explain how to do this.

## The Peakboard application

First, we build a simple Peakboard application to use for our example. The app has a large text box which displays any notifications. It also has a button, which the user can tap, to confirm the notification.

Both of these controls are hidden by default. The text box and button only appear when the app receives a notification. And when the user taps the button, the app re-hides the text box and button.

![image](/assets/2025-10-10/010.png)

We also add a function called `SubmitNotification`. This function accepts a `Message` argument, which contains the notification message. We mark the function as `shared`, so that external apps can call it.

![image](/assets/2025-10-10/020.png)

That's all we need. Now the application can be deployed on a box or on a BYOD instance waiting for incoming messages. The next screenshot shows an incoming message in the running application and how to confirm it.

![image](/assets/2025-10-10/result.gif)

## Calling from C#

As we created a shared function, it automatically exposes an endpoint into the network. The following code shows how to call that function. It's a regular HTTP POST call. The pattern for the URL is `http://<BoxNameOrIP>:40404/api/functions/<FunctionName>`. In the body we need to provide the `Message` value as payload. The endpoint is protected by box credentials. Ideally we create an account on the box that can only call functions and nothing else. In case the credentials get lost they can't be used to access the box as administrators.

{% highlight csharp %}
var url = "http://comicbookguy:40404/api/functions/SubmitNotification";
var payload = "{\"Message\": \"The roof is on fire!\"}";

using (var client = new HttpClient())
{
    // Add Basic Authentication header
    var username = "ExternalCaller";
    var password = "XXX";
    var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

    var content = new StringContent(payload, Encoding.UTF8, "application/json");
    var response = await client.PostAsync(url, content);
    var responseString = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"Response: {response.StatusCode} - {responseString}");
}
{% endhighlight %}

## Calling from Python

In case we want to do the same using Python here's a typical sample for Python users.

{% highlight python %}
import requests
from requests.auth import HTTPBasicAuth

def main():
    url = "http://comicbookguy:40404/api/functions/SubmitNotification"
    payload = {"Message": "The roof is on fire!"}
    username = "ExternalCaller"
    password = "xxx"

    response = requests.post(
        url,
        json=payload,
        auth=HTTPBasicAuth(username, password)
    )

    print(f"Response: {response.status_code} - {response.text}")

if __name__ == "__main__":
    main()
{% endhighlight %}

## Set up a box user

It's very important to keep safety in mind and not use an Administrator account to call the function. Ideally we create a `Caller` role on the user and create a new user bound to this role as shown in the screenshot. Then we can make sure that the caller can only call and nothing else. Calling a function is considered as "Write Data".

![image](/assets/2025-10-10/030.png)

