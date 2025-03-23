---
layout: post
title: Peak-a-Boo! Revealing Peakboard Hub List Data in your Power BI Dashboards
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
Peakboard Hub is not only for Box-related administration tasks. It's often used to store data. Whenever data is stored, data analysis is around the corner. That's our topic today. We will discuss how to use Power BI to consume data that is stored inside  Peakboard Hub.

Before diving into this article, you need to understand some basics around the Peakboard Hub API, especially how to get an API key, how to authenticate against the API backend, how to receive an access token, and how to make the actual API call. These basics are explained in our [Peakboard Hub getting started article](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html). 

The example list we'll use in today's article is called StockInfo, as shown in the following screenshot. It contains three useful columns.

The Power BI solution we're discussing and building is very generic. You can replicate the steps and change the name of the table, and it'll work with any other table perfectly.

![image](/assets/2025-04-18/010.png)

## Set up Power BI table

In Power BI, we create a new table with a blank query:s

![image](/assets/2025-04-18/020.png)

In the query, we can switch to the advanced editor in order to edit the underlying command. This script contains exactly one command that must be modified for the API call. If you're not familiar with these Power BI M queries, see this [video about M queries](https://www.youtube.com/watch?v=N8qYRSqRz84&ab_channel=DhruvinShah).

![image](/assets/2025-04-18/030.png)

Our final command consists of 3 steps:

1. Turn the API key into an access token.
2. Get the list table data as a JSON string.
3. Turn the JSON string into a table.

We will discuss each of the 3 parts separately.

![image](/assets/2025-04-18/040.png)

## Turn the API key into an access token

{% highlight text %}
url = "https://api.peakboard.com/public-api/v1/auth/token",
headers = [ apiKey = "3302004a473aa8a1caade971b74089a3012f54e4" ],
options = [ Headers = headers ],
response = Json.Document(Web.Contents(url, options)),
AccessToken = response[accessToken],
{% endhighlight %}

1. We specify the URL for getting a token.
2. We build a list of headers with a single entry: the API key.
3. We place the list of headers into a list of options.
4. We make an API call to the URL, with the list of options.
5. The response represents a tree of objects in the JSON response string. We retrieve the access token from the response by using brackets.

## Get the list table data as a JSON string

{% highlight text %}
listurl = "https://api.peakboard.com/public-api/v1/lists/list?Name=stockinfo",
listheaders = [ Authorization  = "Bearer " & AccessToken ],
listoptions = [ Headers = listheaders ],
listresponse = Json.Document(Web.Contents(listurl, listoptions)),
{% endhighlight %}

The process for getting the data is similar to the process for getting the access token. But in this case, we need to provide an `Authorization` header instead of the API key. If the request succeeds, `listrespones` will contain the JSON string of the list data.

## Turn the JSON string into a table

Here's what the JSON string looks like. The metadata and the table are distributed over two arrays. The table data is stored as key-value pairs.

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

Here's the code for turning the string into a table:

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

1. We convert the two arrays into two lists containing the headers and the items.
2. We transform both lists with the `List.Transform` function, which applies a function to each element of the list.
  * For the columns, we extract the `name`.
  * For the data, we perform one more step. We get the value of a table cell by doing `item[value]`. Then, we have a list of values. With `Record.FromList`, we transform this list of values into a single record.
3. We transform all the single records into a new table by using another `List.Transform`. 

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



