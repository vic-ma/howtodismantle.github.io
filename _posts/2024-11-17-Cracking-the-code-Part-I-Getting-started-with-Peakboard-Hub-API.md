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

Since its official release in 2022, Peakboard Hub has become more and more popular among customers. Initially, it was only designed to be an administration interface for environments with 20+ boxes. Since then, many API endpoints have been added, and there's also a SaaS offering for customers who don't want to host their Hub on premises.

By end of the 2024, Peakboard introduced an official API, which lets you connect Peakboard Hub to countless other systems, especially in the cloud. Today, we'll kick off a new series of articles that demonstrate what you can do with the Peakboard Hub API and how to integrate it.

## The API key

The first thing we need is a Peakboard Hub API key.
An API key is related to a user group. So an API key can only access the lists, boxes, alerts, etc. that are available to the corresponding user group.

To add a new API key, we go to the user groups of an organization and look for the key list. Then, we click **Add**.

![image](/assets/2024-11-17/010.png)

We define the scope of the key (e.g. restricted to only reading lists), along with the expiration period. Although it's possible to generate a key that never expires, it's best practice to have an expiration date and renew the key on a regular basis.

![image](/assets/2024-11-17/020.png)

After creating the new key, we copy it.

![image](/assets/2024-11-17/030.png)

## Authentication

Before we can make our first API call, we must turn our API key into an access token. To do that, we use the public API endpoint `/public-api/v1/auth/token`.

It requires us to submit our API key in the header of the request. If successful, it returns a JSON string that contains an `accessToken` that we can use later.

The following code demonstrates the authentication process in C#. Feel free to download the whole [CS file](/assets/2024-11-17/PeakboardHubAPIPart1.cs) for this example. We use the NuGet package Newtonsoft.Json to perform the JSON operations.

Here's how the script works:

1. Add the key to the header.
1. Call the `auth/token` endpoint.
1. Get the token from the response.
1. Put the token in the header for future calls. Remove the API key from the header for future calls, because we only need it for the `auth/token` endpoint.

After this sequence, the `HttpClient` object is ready to be used with any API call that is within the scope of the initial API key.

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
else {
Console.WriteLine("Error during authentification");
Console.WriteLine(responseBody.ToString());
return;
}

    // Do something useful and call a real API

    Console.WriteLine("Finished, yay!");

}
{% endhighlight %}

## Actual call

For the actual call, we reuse the client object. In the following example, we list all boxes that are registered in the Hub by calling the `/public-api/v1/box` endpoint. We then loop over the deserialized JSON string

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

The console output should look like this. We note, that most boxes except one is offline.

![image](/assets/2024-11-17/040.png)

## List of endpoints

Here is a list of endpoints that are currently available in version 1 of the Peakboard Hub API. We go through all these endpoints in various articles and various environments.

| Endpoint          | Op.           | Description​                               | More Information​​                                                                                                 |
| ----------------- | ------------- | ------------------------------------------ | ------------------------------------------------------------------------------------------------------------------ |
| /v1/auth/token    | Get           | Turns the API key into an access token     | see this article                                                                                                   |
| -------------     | ------------- | -------------                              | -------------                                                                                                      |
| /v1/box           | Get           | Lists all boxes within the group           | see this article                                                                                                   |
| /v1/box/functions | Get           | List all shared function of a box          | [Calling function remotely](/Cracking-the-code-Part-II-Calling-functions-remotely.html)                            |
| /v1/box/function  | Post          | Executes a shared function on a box        | [Calling function remotely](/Cracking-the-code-Part-II-Calling-functions-remotely.html)                            |
| -------------     | ------------- | -------------                              | -------------                                                                                                      |
| /v1/box/lists     | Get           | List all lists of a box                    |                                                                                                                    |
| /v1/box/lists     | Put           | Changes data in a list on a box            |                                                                                                                    |
| /v1/box/lists     | Delete        | Deletes records of a list on a box         |                                                                                                                    |
| /v1/box/lists     | Post          | Adds a data row to a list on a box         |                                                                                                                    |
| -------------     | ------------- | -------------                              | -------------                                                                                                      |
| /v1/box/variables | Get           | List all variables of a box                |                                                                                                                    |
| /v1/box/variables | Put           | Changes a variable value on a box          |                                                                                                                    |
| -------------     | ------------- | -------------                              | -------------                                                                                                      |
| /v1/lists         | Get           | Lists all lists on the Hub                 | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/list    | Post          | Returns Hub list data by using SQL         | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/list    | Get           | Returns Hub list data                      |                                                                                                                    |
| /v1/lists/items   | Post          | Add a new record to a Hub list             | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/items   | Put           | Changes the data of a record in a Hub list | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
| /v1/lists/items   | Delte         | Deletes a record from a Hub list           | [Reading and writing Hub lists](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html) |
