---
layout: post
title: Sssslithering Through APIs - Python Unleashed for Peakboard Hub
date: 2023-03-01 01:00:00 +0200
tags: api peakboardhub peakboardhubapi
image: /assets/2025-01-20/title.png
image_header: /assets/2025-01-20/title_landscape.png
read_more_links:
  - name: Cracking the code - Part I - Getting started with Peakboard Hub API
    url: /Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html
  - name: Cracking the code - Part II - Calling functions remotely
    url: /Cracking-the-code-Part-II-Calling-functions-remotely.html
downloads:
  - name: PeakboardHubAPIPlayground.py
    url: /assets/2025-01-20/PeakboardHubAPIPlayground.py
---
A couple of weeks ago, we gave an [introduction to the Peakboard Hub API](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html). We also have a guide for using the API to [manage lists in Peakboard Hub](/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html). This article is all about using the Peakboard Hub API in Python. Let's get started.

![image](/assets/2025-01-20/005.png)

## Authentication

Here's how to handle Peakboard Hub API authentication in Python:

{% highlight python %}
import requests
import sys
import pandas

BaseURL = "http://api.peakboard.com";
APIKey = "XXX";

response = requests.get(BaseURL + "/public-api/v1/auth/token", headers={'apiKey': APIKey})

if response.status_code != 200:
    sys.exit("Unable to obtain a access token, so quitting") 

accesstoken = response.json()["accessToken"]
print("Succesfully authorized until " + response.json()["validUntill"])

mySession = requests.Session()
mySession.headers.update({"Authorization": "Bearer " + accesstoken})
{% endhighlight %}

We use the following imports:
* `requests`, for API calls
* `pandas`, for table formatting
* `sys`, for program termination

The Peakboard Hub API's base URL changes based on the Peakboard Hub instance we're addressing. In our script, we're using the standard Hub Online. See our [introductory article](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html) to learn more about base URLs and authentication.

We submit our API key in the header of a request to the `/public-api/v1/auth/token` endpoint. Then, we extract the access token from the response body and build a session instance with it. For all future calls, we won't need to worry about authentication, so long as we use the session instance. 

## Get all lists

To get all the available lists in our Peakboard Hub, we send a `GET` request to the `/public-api/v1/lists` endpoint. Then, we loop over the response body's JSON string in order to print all the table names.

{% highlight python %}
# Get all lists

response = mySession.get(BaseURL + "/public-api/v1/lists")

if response.status_code != 200:
    sys.exit("Unable to obtain list information")

for item in response.json():
    print(f"Table found: {item['name']}")
{% endhighlight %}

![image](/assets/2025-01-20/010.png)

## Get list data

To get the data in a list, we send a `GET` request to the `/public-api/v1/lists/list` endpoint, with a query string that specifies the table name and sort order.

After we get the result, we turn the columns and records of the table into collections. We print the formatted table by using the `pandas` library: `pandas.DataFrame(...)`.

{% highlight python %}
# Get data of a list

response = mySession.get(BaseURL + "/public-api/v1/lists/list?Name=stockinfo&SortColumn=MaterialNo&SortOrder=Asc")

if response.status_code != 200:
    sys.exit("Unable to obtain list data")


columns = [col['name'] for col in response.json()['columns']]
items = [{entry['column']: entry['value'] for entry in item} for item in response.json()['items']]

print(pandas.DataFrame(items, columns=columns))
{% endhighlight %}

![image](/assets/2025-01-20/020.png)

## Create a record

To create a record, we send a `POST` request to the `/public-api/v1/lists/items` endpoint. The following example code shows how to construct a body with the name of the list and the data to be added. If the call succeeds, we read the ID of the record from the response body's JSON string.

{% highlight python %}
# create a new record

body = {
    "listName": "stockinfo",
    "data": {
        "MaterialNo": "0815",
        "Quantity": 5,
        "locked": False
    }
}

response = mySession.post(BaseURL + "/public-api/v1/lists/items", json=body)

if response.status_code != 200:
    sys.exit("Unable to create a new record. Return is " + response.reason)

id = response.json()["addedItem"][0]["value"]
print("New record added under ID " + str(id))#
{% endhighlight %}

## Edit a record

To edit a record, we send a `PUT` request to the `/public-api/v1/lists/items` endpoint. In the request body, we provide the following properties:

* `listName` - The name of the list we want to edit.
* `rowId` - The ID of the record we want to edit.
* `data` - The column name and new data for that column.

{% highlight python %}
# Edit a record

body = {
  "rowId": id,
  "listName": "stockinfo",
  "data": {
    "Quantity": 30
  }
}

response = mySession.put(BaseURL + "/public-api/v1/lists/items", json=body)

if response.status_code != 200:
    sys.exit("Unable to change a record. Return is " + response.reason)

print(f"Record with id {id} was changed")
{% endhighlight %}

## Delete a record

To delete a record, we send a `DELETE` request to the `/public-api/v1/lists` endpoint. We provide the name of the list and the record ID to be deleted.

{% highlight python %}
# delete a record

body = {
  "listName": "stockinfo",
  "rowId": id
}

response = mySession.delete(BaseURL + "/public-api/v1/lists/items", json=body)

if response.status_code != 200:
    sys.exit("Unable to delete a record. Return is " + response.reason)

print(f"Record with id {id} was deleted")
{% endhighlight %}

## Do crazy stuff with SQL

A special endpoint is `/public-api/v1/lists/list`, which takes a `POST` request. This endpoint allows us to execute a SQL command. The cool thing is that we can do data aggregation with it and let the database do all the work. Depending on the use case, this can dramatically reduce the amount of data we receive.

Let's assume that we're not interested in every record in our `stockinfo` table. We only want to know how many records in the table are marked with `locked=true` and how many are marked with `locked=false`. This is the SQL command that gets exactly this information:

{% highlight sql %}
select Locked, count(*) as Counter from stockinfo group by locked
{% endhighlight %}

And here's how to use this SQL command with the endpoint to get a table of aggregated data back. We use the `pandas` function to print out a formatted result table.

{% highlight python %}
# Get table data with the help of SQL command

body = {
  "sql": "select Locked, count(*) as Counter from stockinfo group by locked"
}
response = mySession.post(BaseURL + "/public-api/v1/lists/list", json=body)

if response.status_code != 200:
    sys.exit("Unable to obtain list data")

columns = [col['name'] for col in response.json()['columns']]
items = [{entry['column']: entry['value'] for entry in item} for item in response.json()['items']]

print(pandas.DataFrame(items, columns=columns))
{% endhighlight %}

Here's what the output looks like:

![image](/assets/2025-01-20/030.png)