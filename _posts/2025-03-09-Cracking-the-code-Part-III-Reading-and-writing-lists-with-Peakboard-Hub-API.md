---
layout: post
title: Cracking the code - Part III - Reading and writing lists with Peakboard Hub API
date: 2023-03-01 03:00:00 +0200
tags: api peakboardhub peakboardhubapi
image: /assets/2025-03-09/title.png
read_more_links:
  - name: Peakboard Hub API Swagger portal
    url: https://api.peakboard.com/public-api/index.html
  - name: Cracking the code - Part I - Getting started with Peakboard Hub API
    url: /Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html
  - name: Cracking the code - Part II - Calling functions remotely
    url: /Cracking-the-code-Part-II-Calling-functions-remotely.html
---

Welcome to the third part of our Peakboard Hub API series. To learn the basics, like how to get an API key and access token, see our [getting started](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html) article.

A common use case for Peakboard Hub is using the Hub for data storage. That's why the Hub API has endpoints for reading and writing data to a Hub list. We will discuss these endpoints in this article.

| Endpoint          | Method   | Description                                  |
| ----------------- | -------- | -------------------------------------------- |
| `/v1/lists`       | `GET`    | Return a list of all lists in the Hub.       |
| `/v1/lists/list`  | `GET`    | Return a Hub list.                           |
| `/v1/lists/list`  | `POST`   | Return Hub list data by using a SQL statement.|
| `/v1/lists/items` | `POST`   | Add a new record to a Hub list.              |
| `/v1/lists/items` | `PUT`    | Change the data of a record in a Hub list.   |
| `/v1/lists/items` | `DELETE` | Delete a record from a Hub list.             |

In this article, we will use the `StockInfo` list as an example.

![image](/assets/2025-03-09/010.png)

The correct URL prefix (origin) depends on the version of Peakboard Hub that you're using. This is the correct prefix for Peakboard Hub Online:

{% highlight url %}
https://api.peakboard.com
{% endhighlight %}

You can also try out the different endpoints in the [Swagger portal](https://api.peakboard.com/public-api/index.html).

## List all lists with `GET /v1/lists`

This endpoint returns a list of all available lists that are within the scope of the current API key context. Here is an example response:

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

## Get the data with `GET /lists/list`

Using `GET /lists/list` is the easiest way to access the contents of a list. Here are the query parameters:

| Parameter    | Description                                         |
| ------------ | --------------------------------------------------- |
| `Name`       | The name of the list.                               |
| `SortColumn` | The name of the column to use for sorting.          |
| `SortOrder`  | Either `Asc` for ascending or `Desc` for descending. |
| `SkipRows`   | Omit a certain number of rows to enable pagination. |
| `MaxRows`    | The maximum number of rows to return.               |

Let's build an example call that uses an `HTTP GET` request to get the row with the highest material number:

{% highlight url %}
/v1/lists/list?Name=Stockinfo&SortColumn=MaterialNo&SortOrder=Desc&MaxRows=1
{% endhighlight %}

Here's the response to that call. It consists of an array of columns, including the metadata under the `columns` node. Under the `items` node, there's an array of data rows. It's a name-value pair array.

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

## SQL access to the data

The same endpoint, `/lists/list`, can also be used in a `POST` call to submit a SQL statement and let the Peakboard Hub Database do some work for you. This is the perfect way to aggregate the data before it's returned to you. That way, there's no need to transfer large batches of data when you only need an aggregated view.

Suppose we want to know the number of locked and unlocked stock records from our example table. Here is the correct SQL:

{% highlight sql %}
select Locked, count(*) as Counter from stockinfo group by locked
{% endhighlight %}

So, we wrap this SQL statement into a JSON envelope and submit it in the body of a `POST` request to the API endpoint:

{% highlight json %}
{
  "sql": "select Locked, count(*) as Counter from stockinfo group by locked"
}
{% endhighlight %}

Here's the response:

{% highlight json %}
{
  "columns": [
    {
      "name": "Locked",
      "elementName": "Locked",
      "type": "Boolean"
    },
    {
      "name": "Counter",
      "elementName": "Counter",
      "type": null
    }
  ],
  "items": [
    [
      {
        "column": "Locked",
        "value": false
      },
      {
        "column": "Counter",
        "value": 1
      }
    ],
    [
      {
        "column": "Locked",
        "value": true
      },
      {
        "column": "Counter",
        "value": 3
      }
    ]
  ]
}
{% endhighlight %}

## Add new data with `POST /v1/lists/items`

The `/v1/lists/items` endpoint can be used for any sort of data manipulation. A `POST` call to this endpoint will create a new record. Here's an example request body for record creation:

{% highlight json %}
{
    "listName": "stockinfo",
    "data": {
        "MaterialNo": "0815",
        "Quantity": 5,
        "locked": false
    }
}
{% endhighlight %}

After a successful record creation, the API returns the entire record, including its ID, which identifies the record.

{% highlight json %}
{
  "addedItem": [
    {
      "column": "ID",
      "value": 13
    },
    {
      "column": "MaterialNo",
      "value": "0815"
    },
    {
      "column": "Quantity",
      "value": 5
    },
    {
      "column": "Locked",
      "value": false
    }
  ]
}
{% endhighlight %}

## Change data

To change an existing record in a list, we use the same endpoint as for data creation: `/v1/lists/items`. However, we use `PUT` instead of `POST`. The body is slightly different, because we need to provide the ID of the record to be changed. This example shows how to set the `Quantity` field to a new value:

{% highlight json %}
{
  "rowId": 13,
  "listName": "stockinfo",
  "data": {
    "Quantity": 30
  }
}
{% endhighlight %}

If the action is successful, the response body will contain the corresponding state, to confirm:

{% highlight json %}
{
  "state": "Success"
}
{% endhighlight %}

## Delete data

To delete a record, we use an `HTTP DELETE` request. In the request body, we provide the name of the list and the ID of the record to be deleted:


{% highlight json %}
{
  "listName": "stockinfo",
  "rowId": 13
}
{% endhighlight %}

The confirmation for the Peakboard server works similarly to the change of data:

{% highlight json %}
{
  "state": "Success"
}
{% endhighlight %}
