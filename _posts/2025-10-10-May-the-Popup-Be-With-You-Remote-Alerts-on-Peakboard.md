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
We add a large text control, which we use to display the notification messages. Then, we add a button that the user can tap, which the user can tap to dismiss the notification.

We make both controls hidden by default. The text box and button only appear when the app receives a notification (see the `SubmitNotification` function below).

To make the button control work, we configure its *Tapped* script to do the following:
1. Make the text control hidden.
1. Make the button control hidden.

![image](/assets/2025-10-10/010.png)

### Add notification function

Now, let's create a function that external apps can use to send a notification. We create a function called `SubmitNotification`. This function accepts a `Message` parameter, which contains the notification message. When the function is called, it does the following:
1. Make the button control visible.
1. Make the text control visible.
1. Set the text control to the `Message` parameter.

We also mark the function as a *Shared function*, so that external apps can call it.

![image](/assets/2025-10-10/020.png)

### Deploy it

That's all we need for the app. Now, we deploy the app to a Box (or BYOD instance), and it waits for an external application to call the `SubmitNotification` function.

## Create a new Box user

In order for our external application to call the `SubmitNotification`, it needs to authenticate itself to the Peakboard Box. You should not use your Box's administrator account for this purpose, because it is much more powerful than what you need.

Instead, we go to our Peakboard Box settings and we [create a new user with a new role](https://help.peakboard.com/administration/en-user-administration.html) that only lets them call functions. Calling a function a part of the *Read Data* and *Write Data* permissions.

![image](/assets/2025-10-10/030.png)

## Create an external application

Let's create an external application to send a notification to our Peakboard app.

### Build a C# program

We want to call the `SubmitNotification` function. Because we marked it as a shared function, the Peakboard app automatically exposes an HTTP endpoint on our local network.

The URL for a function's endpoint looks like this:
```url
http://<BoxNameOrIP>:40404/api/functions/<FunctionName>
```

You can also find the exact endpoint URL in the function script settings, beside the *Shared function* checkbox that you ticked earlier.

The endpoint is protected, so we need to authenticate ourselves with [Basic access authentication](https://en.wikipedia.org/wiki/basic_access_authentication). We use the credentials for the `ExternalCaller` user that we created earlier. This is much more safe than using the administrator account's credentials.

In the request body, we provide the value for the `Message` parameter. In this case, we use the message, `The roof is on fire!`.

{% highlight csharp %}
var url = "http://comicbookguy:40404/api/functions/SubmitNotification";
var payload = "{\"Message\": \"The roof is on fire!\"}";

using (var client = new HttpClient())
{
    // Add the Basic Authentication header.
    var username = "ExternalCaller";
    var password = "XXX";
    var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

    // Set the request body.
    var content = new StringContent(payload, Encoding.UTF8, "application/json");

    // Wait for the response and print it once it arrives.
    var response = await client.PostAsync(url, content);
    var responseString = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"Response: {response.StatusCode} - {responseString}");
}
{% endhighlight %}

### Alternative in Python

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


## Conclusion

Now, when we run our C# application, which calls the `SubmitNotification` function, we can see that the notification appears on screen:

![image](/assets/2025-10-10/010.png)