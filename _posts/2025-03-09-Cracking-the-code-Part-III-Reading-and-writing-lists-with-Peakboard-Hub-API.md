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
Welcome to the third part of Peakboard Hub API series. If you're not familiar with the basics, like how to get an API key and how to use it to obtain and handle an access token, first check out the [Getting started](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html) article.

One of the typical use cases for the Peakboard Hub is using the Hub as a data storage. That's why there are a couple of functions available to read and write the data of a hub list which we will discuss in this article.

| Endpoint            | Op.           | Description |
| ------------------- | ------------- | ------------- |
| `/v1/lists`         | `GET`         | List all lists on the Hub  |
| `/v1/lists/list`    | `GET`         | Return Hub list  |
| `/v1/lists/list`    | `POST`        | Return Hub list data by using SQL |
| `/v1/lists/items`   | `POST`        | Add a new record to a Hub list.            | 
| `/v1/lists/items`   | `PUT`         | Change the data of a record in a Hub list. | 
| `/v1/lists/items`   | `DELETE`      | Delete a record from a Hub list.           | 

In this article we will use the stockinfo list as a sample.

![image](/assets/2025-03-09/010.png)

The correct prefix to form the whole URL depends on the Peakboard Hub you're using. In case of Peakboard Hub Online it's "https://api.peakboard.com". The Swagger portal for playing around with the function can be found [here](https://api.peakboard.com/public-api/index.html). 

## List all lists with GET `/v1/lists`

Nothing important to say here. This function returns a list of all available lists that are within the scope of the current API key context.
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

Using GET `/lists/list` is the easiest way to access the data content of a list. Here are the query parameters:

- Name is the name of the list
- SortColumn is the name of the column to be used for sorting
- SortOrder can be Asc or Desc
- SkipRows is omiting a certain number of rows to enable pagination
- MaxRows is the maximal number of rows.

Let's a build a sample call bei using HTTP GET to get one single row with the highest material number:

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

## SQL access to the data

The same endpoint `/lists/list` can be also used in a POST call to submit a real SQL statement and let the Peakboard Hub Database do some work for you. This a perfect way to aggregate the data before it's transferred. So no need to transfer large batches of data when you only need an  aggregated view. Let's imagine we want to know only the number of locked and unlocked stock records from our sample table. The correct SQL would be 

{% highlight sql %}
select Locked, count(*) as Counter from stockinfo group by locked
{% endhighlight %}

So we just wrap this SQL statement into a JSON envelope and submit it the HTTP body to the API endpoint.

{% highlight json %}
{
  "sql": "select Locked, count(*) as Counter from stockinfo group by locked"
}
{% endhighlight %}

Here's the result.

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

## Adding new data by posting to `/v1/lists/items`

The endpoint `/v1/lists/items` can be used for any data manipulation. A POST call to this endpoint will create a new record.
Here's the body for the record creation:


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

After successful record creation the API returns the whole record including its ID to identify the record.

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

## Changing data 

To change an existing record in a list we use the same endpoint as for the data creation: `/v1/lists/items`. However we use PUT instead of POST. The body is slightly different as we need to provide the ID of the record to be changed. The sample shows how to set the "Quantity" field to a new value.

{% highlight json %}
{
  "rowId": 13,
  "listName": "stockinfo",
  "data": {
    "Quantity": 30
  }
}
{% endhighlight %}

If the acction is succesful the response body is filled with the cooresponding state to confirm.

{% highlight json %}
{
  "state": "Success"
}
{% endhighlight %}

## Deleting data

For deleting a record we use a HTTP DELETE command and provide the name of the list and the ID of the record to be deleted in the HTTP body.

{% highlight json %}
{
  "listName": "stockinfo",
  "rowId": 13
}
{% endhighlight %}

The confirmation for the Peakboard servern works similiar to the Change of data.

{% highlight json %}
{
  "state": "Success"
}
{% endhighlight %}

