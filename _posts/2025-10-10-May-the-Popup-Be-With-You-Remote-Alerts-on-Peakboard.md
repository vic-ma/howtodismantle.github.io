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
In a previous article, we discussed how to use the Peakboard Hub API to [send a notification to a Peakboard Box](/Cracking-the-code-Part-II-Calling-functions-remotely.html). This method is very useful when the Peakboard Box (or BYOD instance) is not directly reachable from the caller, or when the IP address is unknown. It works even with remote Boxes, so long as they are connected to Peakboard Hub Online.

But what if everything is in a single facility, and we can access the Box directly? In that case, we don't need to use the Hub API at all. We can just send our notification directly to the Box. And in today's article, we'll explain how to do this.

## Build the Peakboard app

First, we build a simple Peakboard application that can show a notification to the user. The user can also tap a button to dismiss the notification.

The following video shows what the finished app looks like, when an external application sends a notification, and the user taps on the button to dismiss the notification:

![image](/assets/2025-10-10/result.gif)

### Add controls
We add a large text control, which we will use to display notification messages. Then, we add a button that the user can tap, to confirm the notification.

We make both controls hidden by default. The text box and button only appear when the app receives a notification (see the `SubmitNotification` function below).

To make the confirmation button work, we configure its *Tapped* script to do the following:
1. Make the text control hidden.
1. Make the button control hidden.

![image](/assets/2025-10-10/010.png)

### Add notification function

The app has a function called `SubmitNotification`. This function accepts a `Message` parameter, which contains the notification message. When the function is called, it does the following:
1. Make the button control visible.
1. Make the text control visible.
1. Set the text control to the `Message` parameter.

We also mark the function as a *Shared function*, so that external apps can call it.

![image](/assets/2025-10-10/020.png)

### Deploy it

That's all we need for the app. Now, we deploy the app to a Box (or BYOD instance), and it waits for an external application to call the `SubmitNotification` function.

## Create an external application

Let's create an external application to send a notification to the app.

### Build a C# program

We want to call the `SubmitNotification` function. Because we marked it as a shared function, the Peakboard app automatically exposes an HTTP endpoint on our local network.

The following code shows how to call that function. It's a regular HTTP POST call. The pattern for the URL is this:
```url
http://<BoxNameOrIP>:40404/api/functions/<FunctionName>
```

The endpoint is protected by Box credentials. Ideally, we create an account on the Box that can only call functions and nothing else. That way, in case the credentials are stolen, they can't be used to gain administrator access to the Box.

In the request body, we provide the `Message` value.

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

### Calling from Python

Of course, the app doesn't have to be in C#. Here's an example of same app, but written in Python:

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

## Set up a Box user

It's very important to keep safety in mind and not use an Administrator account to call the function. Ideally we create a `Caller` role on the user and create a new user bound to this role as shown in the screenshot. Then we can make sure that the caller can only call and nothing else. Calling a function is considered as "Write Data".

![image](/assets/2025-10-10/030.png)

## Conclusion

