---
layout: post
title: Dismantle Matomo Reporting API with XML processesing 
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
Beside Google Analytics, Matomo is the gold standard when it comes to tracking and analyzing visitosr on a website. This article covers a short introduction on the Matomo Reporting API which allows to access the analytics data through different layers of aggregation. The best way to access these data is by using the XML data source in Peakboard. So we will covers some basics around this as well.

## Matomo backend

For the API calls we will need an API token. You need to be an Matomo administrator to issue one. In the Matomo backend just goto to Administration -> Personal -> Security and scroll down ot the bottom of the page. Here you can issue a new token just by providing a small description. Store the token somewhere for later use.

![image](/assets/2023-04-10/010.png)

The best way to find your way around is to to use the API page in the administration backend: Platform -> API.
There are several modules installed which refer to different topics in Matomo, e.g. goals, forms, pages.
We will have a look at a very simple API module called Live to start with. It returns the number of visitors and pages in the last minutes and it can be used to show some kind of real time basic traffic on the site.   

![image](/assets/2023-04-10/020.png)

Here's the call for the counters of the last 30 minutes. Please note that you have to provide the API token for authentification. No more other authentification necessary, which is very covenient. You can just play around with he API calls by click on the links provided in the API overview page.

{% highlight url %}
https://matomo.peakboard.com/index.php?module=API&
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


