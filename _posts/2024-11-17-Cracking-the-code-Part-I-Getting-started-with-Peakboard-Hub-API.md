---
layout: post
title: Cracking the code - Part I - Getting started with Peakboard Hub API 
date: 2023-03-01 12:00:00 +0200
tags: api peakboardhub
image: /assets/2024-11-17/title.png
read_more_links:
  - name: Cracking the code - Part II - Calling functions remotely
    url: /Cracking-the-code-Part-II-Calling-functions-remotely.html
  - name: Cracking the code - Part III - Reading and writing lists with Peakboard Hub API
    url: /Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html
downloads:
  - name: PeakboardHubAPIPart1.cs
    url: /assets/2024-11-17/PeakboardHubAPIPart1.cs
---
Since its official release back in 2022 the Peakboard Hub got more and more popular among customers. Initially it was designed only to build an administration interface for a environment with 20+ boxes. Since then there has been not only an increasing number function added but there's also a Saas offering available for customers who don't want to host their hub on prem.

As by end of the year 2024 Peakboard introduced an official API that opens a huge number of possibilities to connect the Peakboard Hub to uncountable other systems, especially in the cloud. With today's article we kick off a new series of articles about what you can do with the Peakboard Hub API and how to integrate it.

## The API key

The first thing we nedd to get things started is an API key.
The idea is, that the key is related to a user group. So only the lists, boxes, alerts, etc. are accessible through the key that are marked to be available in the corresponding user group.

To add a new key we move to the groups of an organsiation and find the key list there.

![image](/assets/2024-11-17/010.png)

When adding a new key, we can define the scope of the key (e.g. restricted to only reading list), along with the validity period. Although it's an option to generate a key that never expires, it's a good habbit to restrict the time and then renew the key on a regular basis.

![image](/assets/2024-11-17/020.png)

After creating the new key it can be copied to the clipboard.

![image](/assets/2024-11-17/030.png)

## Authenfication

Before we can do our first API call, we must turn the API key into an access to token. There's a public API function available to do that: "/public-api/v1/auth/token". It requires to submit the API key in the header of the request. If successful, it returns a JSON string the contains the "accessToken" that can be sued later.

The following code shows how to do that in C#. Feel free to download the whole [cs file](/assets/2024-11-17/PeakboardHubAPIPart1.cs) for this example. We use the nuget package Newtonsoft.Json to do the JSON operations.

- We add the key to the header
- Call the token function
- Get the token from he response and put it in te header for the next call
- remove the API key from the header, because we only needed it for the first call

After that squence, the HttpClient object is ready to be used with any other API call that is within the scope of the initial API key.

{% highlight cs %}
lusing Newtonsoft.Json.Linq;

const string BaseURL = "http://hub.peakboard.com";
const string APIKey = "XXX";

using (HttpClient client = new HttpClient())
{
    client.DefaultRequestHeaders.Add("apiKey",APIKey);
    HttpResponseMessage response = client.GetAsync(BaseURL + "/public-api/v1/auth/token").Result;
    var responseBody = response.Content.ReadAsStringAsync().Result;
    if (response.IsSuccessStatusCode)
    {
        var rawdata = JObject.Parse(responseBody);
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + rawdata["accessToken"]?.ToString());
        client.DefaultRequestHeaders.Remove("apiKey");
        Console.WriteLine("Access Token succesfully received " + rawdata["accessToken"]?.ToString());
    }
    else                {
        Console.WriteLine("Error during authentification");
        Console.WriteLine(responseBody.ToString());
        return;
    }

    // Do something useful and call a real API

    Console.WriteLine("Finished, yay!");                    
}
{% endhighlight %}

## Actual call

The actual call is straight forward just by re-using the client object. In this example we just list all boxes that are registered within the hub by calling "/public-api/v1/box" and then loop over the de-serialized JSON string

{% highlight cs %}
    response = client.GetAsync(BaseURL + "/public-api/v1/box").Result;
    responseBody = response.Content.ReadAsStringAsync().Result;

    if (response.IsSuccessStatusCode)
    {
        JArray rawlist = JArray.Parse(responseBody);
        Console.WriteLine($"Found {rawlist.Count} boxes");
        foreach(var row in rawlist)
            Console.WriteLine($"{row["name"]} - {row["boxID"]} - {row["runtimeVersion"]} - {row["state"]}");
    }
    else
        Console.WriteLine("Error during call -> " + response.StatusCode + response.ReasonPhrase);
{% endhighlight %}

The console output should look like this. We note, that most boxes except one offline.

![image](/assets/2024-11-17/040.png)

## List of functions

Here is a list of functions that are currently available with version 1.


| Function | Op. | Description​ | More Information​​ |
| ------------- | ------------- | ------------- | ------------- |
| /v1/auth/token  | Get | Turns the API key into an access token | see this article |
| ------------- | ------------- | ------------- | ------------- |
| /v1/box  | Get | Lists all boxes within the group | see this article |
| /v1/box/functions  | Get | List all shared function of a box | [Calling function remotely](/Cracking-the-code-Part-II-Calling-functions-remotely.html) |
| /v1/box/function | Post | Executes a shared function on a box | [Calling function remotely](/Cracking-the-code-Part-II-Calling-functions-remotely.html) |
| ------------- | ------------- | ------------- | ------------- |
| /v1/box/lists  | Get | List all lists of a box |  |
| /v1/box/lists  | Put | Changes data in a list on a box |  |
| /v1/box/lists  | Delete | Deletes records of a list on a box |  |
| /v1/box/lists  | Post | Adds a data row to a list on a box |  |
| ------------- | ------------- | ------------- | ------------- |
| /v1/box/variables  | Get | List all variables of a box |  |
| /v1/box/variables  | Put | Changes a variable value on a box |  |
| ------------- | ------------- | ------------- | ------------- |
| /v1/lists  | Get | Lists all lists on the Hub | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/list | Post | Returns Hub list data by using SQL | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/list | Get |  Returns Hub list data |  |
| /v1/lists/items  | Post | Add a new record to a Hub list | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/items  | Put | Changes the data of a record in a Hub list | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/items  | Delte | Deletes a record from a Hub list | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |


