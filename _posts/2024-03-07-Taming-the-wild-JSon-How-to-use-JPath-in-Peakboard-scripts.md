---
layout: post
title: Taming the wild JSon - How to use JPath in Peakboard scriptsg
date: 2023-03-01 02:00:00 +0200
tags: api basics
image: /assets/2024-03-07/title.png
read_more_links:
  - name: Reporting API Reference
    url: https://developer.matomo.org/api-reference/reporting-api
downloads:
  - name: JSonJPathExamples.pbmx
    url: /assets/2024-03-07/JSonJPathExamples.pbmx
---



![image](/assets/2023-08-29/010.png)


{% highlight url %}
https://matomo.mywebsite.com/index.php?module=API&
  method=Live.getCounters&idSite=2&lastMinutes=30&
  format=xml&token_auth=MyToken
{% endhighlight %}
