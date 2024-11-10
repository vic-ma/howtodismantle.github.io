---
layout: post
title: Cracking the code - Part III - Reading and writing lists with Peakboard Hub API
date: 2023-03-01 03:00:00 +0200
tags: api peakboardhub
image: /assets/2025-02-05/title.png
read_more_links:
  - name: Cracking the code - Part I - Getting started with Peakboard Hub API
    url: /Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html
  - name: Cracking the code - Part II - Calling functions remotely
    url: /Cracking-the-code-Part-II-Calling-functions-remotely.html
downloads:
  - name: PeakboardHubAPIPart1.cs
    url: /assets/2024-11-17/PeakboardHubAPIPart1.cs
---
Welcome to the third part of Peakboard Hub API series. If you're not familiar with the basics, like how to get an API key and how to use it to obtain and handle an access token, first check out the [Getting started](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html) article.

One of the typical use cases for the Peakboard Hub is using the Hub as a data storage. That's why there are a couple of functions available to read and write the data of a hub list which we will discuss in this article.

| Endpoint            | Op.           | Description |
| ------------------- | ------------- | ------------- |
| `/v1/lists`         | `GET`         | List all lists on the Hub  |
| `/v1/lists/list`    | `POST`        | Return Hub list data by using SQL |
| `/v1/lists/list`    | `GET`         | Return Hub list  |
| `/v1/lists/items`   | `POST`        | Add a new record to a Hub list.            | 
| `/v1/lists/items`   | `PUT`         | Change the data of a record in a Hub list. | 
| `/v1/lists/items`   | `DELETE`      | Delete a record from a Hub list.           | 

In this article we will use the stockinfo list as a sample.

![image](/assets/2025-02-05/010.png)

## List all lists with `/v1/lists`

Nothing important to say here. This function resturns a list of all available lists that are within the scope of the current API key context.
The sample shows the return message.

{% highlight json %}
[
  {
    "id": 1085,
    "name": "Booking"
  },
  {
    "id": 1084,
    "name": "StockInfo"
  }
]
{% endhighlight %}

## Get the data with `/lists/list`

Using `/lists/list` is the easiest way to access the data content of a list. Here are the query paramters:

- Name is the name of the list
- SortColumn is the name of the column to be used for sorting
- SortOrder can be Asc or Desc
- SkipRows is omiting a certain number of rows to enable pagination
- MaxRows is the maximal number of rows.

Let's a build a sample call get one single row with the highest material number:

{% highlight url %}
/v1/lists/list?Name=Stockinfo&SortColumn=MaterialNo&SortOrder=Desc&MaxRows=1
{% endhighlight %}

Here's the result of that call. It consists of a arrays of columns including the meta data type under the "columns" node. Under the "items" node we find an array of data rows. It's a name/value pair array.

{% highlight json %}
{
  "columns": [
    {
      "name": "ID",
      "elementName": "ID",
      "type": "Number"
    },
    {
      "name": "MaterialNo",
      "elementName": "MaterialNo",
      "type": "String"
    },
    {
      "name": "Quantity",
      "elementName": "Quantity",
      "type": "Number"
    },
    {
      "name": "Locked",
      "elementName": "Locked",
      "type": "Boolean"
    }
  ],
  "items": [
    [
      {
        "column": "ID",
        "value": 4
      },
      {
        "column": "MaterialNo",
        "value": "4716"
      },
      {
        "column": "Quantity",
        "value": 810
      },
      {
        "column": "Locked",
        "value": true
      }
    ]
  ]
}
{% endhighlight %}

## Get the data with `/lists/list`




