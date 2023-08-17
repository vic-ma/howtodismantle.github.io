---
layout: post
title: Dismantle Matomo Reporting API with XML processing 
date: 2023-03-01 12:00:00 +0200
tags: api basics
image: /assets/2023-08-29/title.png
read_more_links:
  - name: Reporting API Reference
    url: https://developer.matomo.org/api-reference/reporting-api
downloads:
  - name: MatomoBoard.pbmx (API Token is removed)
    url: /assets/2023-08-29/MatomoBoard.pbmx
---
Beside Google Analytics, Matomo is the gold standard when it comes to tracking and analyzing visitors on a website. This article covers a short introduction on the Matomo Reporting API which allows to access the analytics data through different layers of aggregation. The best way to access these data is by using the XML data source in Peakboard. So we will cover some basics around this as well.

## Matomo backend

For the API calls we will need an API token. You need to be a Matomo administrator to issue one. In the Matomo backend just goto to Administration -> Personal -> Security and scroll down ot the bottom of the page. Here you can issue a new token just by providing a small description. Store the token somewhere for later use.

![image](/assets/2023-08-29/010.png)

The best way to find your way around is to to use the API page in the administration backend: Platform -> API.
There are several modules installed which refer to different topics in Matomo, e.g. goals, forms, pages.
We will have a look at a very simple API module called Live to start with. It returns the number of visitors and pages in the last minutes and it can be used to show some kind of real time basic traffic on the site.   

![image](/assets/2023-08-29/020.png)

Here's the call for the counters of the last 30 minutes. Please note that you have to provide the API token for authentification. No more other authentification necessary, which is very covenient. You can just play around with he API calls by click on the links provided in the API overview page.

{% highlight url %}
https://matomo.mywebsite.com/index.php?module=API&
  method=Live.getCounters&idSite=2&lastMinutes=30&
  format=xml&token_auth=MyToken
{% endhighlight %}

Here's the XML that is returned:

{% highlight xml %}
<result>
	<row>
		<visits>18</visits>
		<actions>40</actions>
		<visitors>16</visitors>
		<visitsConverted>0</visitsConverted>
	</row>
</result>
{% endhighlight %}

The second call we will look at, is the module VisitsSummary with its method getVisits. It allows to query for classical website reports, like give my the numbers of visitors per day for he last 60 days, to turn it into a nice line charts later. Here's the call:

{% highlight url %}
https://matomo.mywebsite.com/index.php?module=API&
  method=VisitsSummary.getVisits&idSite=2&period=day&
  date=last60&format=xml&token_auth=MyToken
{% endhighlight %}

Here's the sample XML:

{% highlight xml %}
<results>
	<result date="2023-06-16">16</result>
	<result date="2023-06-17"/>
	<result date="2023-06-18">1</result>
	<result date="2023-06-19">30</result>
	<result date="2023-06-20">32</result>
    </more data>....
</results>
{% endhighlight %}

## Building the data source in Peakboard Designer

In Peakboard Designer we will process the data with a simple XML data source. The following screenshot shows the first call. The trick is to determine the right path value to find the data you're interested in within the XML data. Just press the value help button for the path. You can navigate though the XML hierarchy and select the right branch and the values you're interested in...

![image](/assets/2023-08-29/030.png)

... and so build a table with one single row:

![image](/assets/2023-08-29/040.png)

We do the same for the second call:

![image](/assets/2023-08-29/050.png)

However the nature of the XML is a little bit different, so we get a table back. The important point here is, that we need to change the data type of the visitors from string (which is default) to number. We will use this table later directly in a chart and only numbers can be properly used for Y-axis.

![image](/assets/2023-08-29/060.png)

Preparing the canvas is straight forward. The real time visitors are presented through a simple text field with title and subtitle (see top left corner). And the chart is a simple spline chart that is bound directly to the second data source. Feel free to just download the pbmx file (see bottom of the page) if you're interested in more details.

![image](/assets/2023-08-29/070.png)

And here's the final result:

![image](/assets/2023-08-29/080.png)



