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
Earlier this year we discussed how to use the [Peakboard Hub API to send a notification from any kind of client to a Peakboard box through the Hub](/Cracking-the-code-Part-II-Calling-functions-remotely.html). That's especially cool when the Peakboard box (or BYOD instance) is not directly reachable from the caller or the IP address is unknown. It works even with remote boxes as long as they are connected to Peakboard Hub Online. But let's assume now, we are only operating within one local facility and the box can be accessed directly without any restriction. In that case, no hub in between is necessary—we can just send our notification directly from the caller to the box without any relay. How to do that we will discuss in today's article.

## The Peakboard application

For our example we need a simple Peakboard application. It contains a big text box for the message and a button to let the user confirm the notification. Both elements are hidden by default, so confirming a message is nothing else than setting the text box and the button back to hidden.

![image](/assets/2025-10-10/010.png)

Besides the two controls we will need a function called `SubmitNotification` which receives a parameter `Message` that contains the actual payload to be presented to the user. The function must be marked as `shared` to allow it to be called from the outside.

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

