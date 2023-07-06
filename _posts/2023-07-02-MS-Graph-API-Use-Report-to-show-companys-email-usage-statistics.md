---
layout: post
title: MS Graph API - Use Report to show company's email usage statistics
date: 2023-07-02 12:00:00 +0200
tags: msgraph
image: /assets/2023-07-02/060.png
---
Graph API offers a ton of monitoring reports to give system admins insights on what is happening in their Office 365 tenant. Please find the official documentation [here](https://learn.microsoft.com/en-us/graph/api/resources/report?view=graph-rest-1.0). In today's article we will have a look at a report that lists the aggregated outlook acticity per day for the whole organisation. [Here](https://learn.microsoft.com/en-us/graph/api/reportroot-getemailactivityusercounts?view=graph-rest-1.0)'s the documentation on that. 

Please make sure to read through the basics of using MS Graph API in Peakboard: [MS Graph API - Understand the basis and get started]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %})

## Configuring the data source

The correct URL for the report is 

{% highlight url %}
https://graph.microsoft.com/beta/reports/microsoft.graph.getEmailActivityCounts(period='D30')
{% endhighlight %}

We are providing an additional parameter "period='D30'" to indicate that we want the statistic data from the last 30 days.
For the data source we use a _App-only_ Graph extension list that doesn't require any user delegation. Depending on how you configured you AD registered app, please make sure to have granted the permission for _Reports.Read.All_ there.

![image](/assets/2023-07-02/010.png)

## Getting the data ready

All the data comes as string data types, so it's hard to to use it in a chart later. We need to add a data flow below the data source to adjust the columns for further usage.

![image](/assets/2023-07-02/020.png)

We convert the two columns _Send_ and _Receive_ into numbers

![image](/assets/2023-07-02/030.png)

and remove all the useless columns except the data and two numbers we're interested in. That's it.

![image](/assets/2023-07-02/040.png)

## Building the chart

A line and area chart is a good choice to display the data. In that case we go for two series. One for the received emails, and one for the sent emails. X axis is always the date.
The confirgration is straight forward as we already prepared the data very well. 

![image](/assets/2023-07-02/050.png)

Let's go ahead snd have a look at the final result:

![image](/assets/2023-07-02/060.png)

* Download [GraphEmailUsageReport.pbmx](/assets/2023-07-02/GraphEmailUsageReport.pbmx)
