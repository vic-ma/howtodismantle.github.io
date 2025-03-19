---
layout: post
title: 2025-04-18 Peak-a-Boo! Revealing Peakboard Hub List Data in your Power BI Dashboards
date: 2023-03-01 03:00:00 +0200
tags: api peakboardhub peakboardhubapi office365
image: /assets/2025-04-18/title.png
image_landscape: /assets/2025-04-18/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Peakboard Hub API Swagger portal
    url: https://api.peakboard.com/public-api/index.html
  - name: YouTube - Getting Started With M Language in Power Query | Basic to Advanced
    url: https://www.youtube.com/watch?v=N8qYRSqRz84&ab_channel=DhruvinShah
  - name: Cracking the code - Part II - Calling functions remotely
    url: /Cracking-the-code-Part-II-Calling-functions-remotely.html
downloads:
  - name: StockInfoPowerBI.pbix
    url: /assets/2025-04-18/StockInfoPowerBI.pbix
---
The Peakboard Hub is not only for box administration tasks. It's often used to store data. Whenever data is stored, data analysis is around the corner. That's our topic today. We will discuss how to use Power BI to consume data that is stored in the Peakboard Hub.
As requirement we need to understand some basics around the Peakboard Hub API, especially how to get an API key, how to authenticate against the API backend, receive an access token and then do the actual call. These basics are explained in the article [Cracking the code - Part I - Getting started with Peakboard Hub API](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html). 

The sample list we're using in today's article is the stockinfo table as shown in the screenshot. It contains three useful columns. The Power BI solution we're discussing and building is very generic. We can use it and just exchange the name of the table and then it will run with any other table completely genericly.

![image](/assets/2025-04-18/010.png)

## Set up Power BI table

In Power BI we create a new table with a blank query.

![image](/assets/2025-04-18/020.png)

In the query we can switch to the advanced editor for the editing the underlying command. This script contains exactly one command that must be modfied for the API call. If you're not familiar with these Power BI M queries, here's a [perfect video](https://www.youtube.com/watch?v=N8qYRSqRz84&ab_channel=DhruvinShah) that explains the concept of M queries.

![image](/assets/2025-04-18/030.png)

Our final command basically consists of 3 parts:

1. Turning the API key into an access token
2. Get the list table data as JSON string
3. process the JSON string into a useful table

![image](/assets/2025-04-18/040.png)

We will discuss these 3 parts separately.

## Turning the API key into an access token

{% highlight text %}
url = "https://api.peakboard.com/public-api/v1/auth/token",
headers = [ apiKey = "3302004a473aa8a1caade971b74089a3012f54e4" ],
options = [ Headers = headers ],
response = Json.Document(Web.Contents(url, options)),
AccessToken = response[accessToken],
{% endhighlight %}

In the first part we define the URL to get the token. We need to build a list of headers with one entry: the API key. This list of headers goes into a list of options, that is used call the API. The variable "response" represents a tree of objects in the JSON response. We can easily address the access token by using brackets.

## Get the list table data as JSON string

{% highlight text %}
listurl = "https://api.peakboard.com/public-api/v1/lists/list?Name=stockinfo",
listheaders = [ Authorization  = "Bearer " & AccessToken ],
listoptions = [ Headers = listheaders ],
listresponse = Json.Document(Web.Contents(listurl, listoptions)),
{% endhighlight %}

The actual call to get the data works very similiar. But in that case we need to provide an "Authorization" header instead of the API key. When everything goes well the JSON string with the actual list data is stored in the "listresponse" variable.

## Process the JSON string into a useful table

As a reminder here's how the JSON string looks like. The meta data and the actual table is distributed over two arrays. The actual table data is stored in key/value pairs.

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

First we need to convert the two arrays into two lists containing the headers and the items. 
Then both lists are transformed by using the "List.Transform" command. It can apply a function to each element of the list. For the columns we just extract the "name". For the actual data we need one more step. The value of a table cell is addressed by using "item[value]". Then we have a list of values. With "Record.FromList" we transform this list of values into a single record. All the single records are transformed into a new table by using another "List.Transform". 

{% highlight text %}
Columns = listresponse[columns],
Items = listresponse[items],
ColumnNames = List.Transform(Columns, each [name]),
RecordsList = List.Transform(Items, each 
    Record.FromList(
        List.Transform(_, (item) => item[value]), 
        ColumnNames
    )
),
ResultTable = Table.FromRecords(RecordsList)
{% endhighlight %}

## Result

The command we created can be easily adjusted by just changing the API key and the name of the table. Then it works with any table.
Here's the complete command as reference and for copy and paste:

{% highlight text %}
let
    url = "https://api.peakboard.com/public-api/v1/auth/token",
    headers = [ apiKey = "3302004a473aa8a1caade971b74089a3012f54e4" ],
    options = [ Headers = headers ],
    response = Json.Document(Web.Contents(url, options)),
    AccessToken = response[accessToken],
    listurl = "https://api.peakboard.com/public-api/v1/lists/list?Name=stockinfo",
    listheaders = [ Authorization  = "Bearer " & AccessToken ],
    listoptions = [ Headers = listheaders ],
    listresponse = Json.Document(Web.Contents(listurl, listoptions)),
    Columns = listresponse[columns],
    Items = listresponse[items],
    ColumnNames = List.Transform(Columns, each [name]),
    RecordsList = List.Transform(Items, each 
        Record.FromList(
            List.Transform(_, (item) => item[value]), 
            ColumnNames
        )
    ),
    ResultTable = Table.FromRecords(RecordsList)
in
    ResultTable
{% endhighlight %}

And here's the final result in Power BI preview:

![image](/assets/2025-04-18/050.png)



