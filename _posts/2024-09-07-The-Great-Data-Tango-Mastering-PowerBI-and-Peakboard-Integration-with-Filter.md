---
layout: post
title: The Great Data Tango - Mastering PowerBI and Peakboard Integration with Filters
date: 2023-03-01 00:00:00 +0200
tags: office365 
image: /assets/2024-09-07/title.png
read_more_links:
  - name: Best Practice - Use Power BI for integrating maps
    url: /best-practice-powerbi-for-map-integration.html
  - name: Power BI Filter API
    url: https://learn.microsoft.com/de-de/power-bi/developer/visuals/filter-api
  - name: Good filter examples on github (in the readme.md)
    url: https://github.com/Microsoft/powerbi-models
downloads:
  - name: PowerBISalesReport.pbmx
    url: /assets/2024-09-07/PowerBISalesReport.pbmx
  - name: SalesReport.pbix
    url: /assets/2024-09-07/SalesReport.pbix
---
A long time ago we discussed how to use [Power BI Maps in Peakboard applications](/best-practice-powerbi-for-map-integration.html). In this aticle we will talk about another apsect of Power BI integration: Filters. The feature of being able to set dynamic filters is available for Peakboard users since Q3 2024.

## requirements and prerequisites

The Power BI sheet we're using can be downloaded [here](/assets/2024-09-07/SalesReport.pbix). It's just a demonstration board based on sales transactions showing charts and tables along with some filters.
To integrate this PBI report in Peakboard, we must first upload it to the the Microsoft Fabric Portal and then follow the steps decribed in the [help](https://help.peakboard.com/controls/Extended/en-power-bi.html) to obtain the Application ID and Tenant ID, along with user credentials to access the hosted Power BI report from within the Pekaboard designer. After access is granted we can choose workspace and report.

![image](/assets/2024-09-07/010.png)

Under the tab 'Report Settings' we can find the option to apply filters to the report. The filters are built as JSON strings.

![image](/assets/2024-09-07/020.png)

## Understanding filters

The above mentioned filters are actually and array of filters wrapped in brackets. The main form of the JSON that be used within the filter property of the PBI control: 

{% highlight json %}
[
      Filter 1 ...
    ,
      Filter 2 ...
]
{% endhighlight %}

A single filter can be either a basic filter (just selecting dictinct values) or a complex filter (having multiple conditions). Here's an example of a basic filter with two distinct values "Canada" and "India". The target describes the table and column within the data PBI model.

{% highlight json %}
{
    "target": {
        "table": "financials",
        "column": "Country"
    },
    "operator": "In",
    "values": ["Canada", "India"]
}
{% endhighlight %}

The complex filters are slightly more complicated as we can provide multiple conditions connected with a logical operator. 

{% highlight json %}
{
    "target": {
        "table": "financials",
        "column": "Units Sold"
    },
    "logicalOperator": "And",
    "conditions": [
        {
            "operator": "GreaterThan",
            "value": "150"
        },
        {
            "operator": "LessThan",
            "value": "201"
        }
    ]
}
{% endhighlight %}

One last sample to show how use date fields in a complex filter.

{% highlight json %}
{
  "target": {
    "table": "Time",
    "column": "Date"
  },
  "logicalOperator": "And",
  "conditions": [
    {
      "operator": "GreaterThan",
      "value": "2014-06-01T07:00:00.000Z"
    }
  ]
}
{% endhighlight %}

The way both basic and complex filters are created and combined with each other is very similiar how a human PBI dashboard user would apply filters. If we keep that in mind it's very easy to find the right JSON to apply on our own PBI report.

## Making filters dynamic

In most cases we need a dynamic filter rather than just a fixed JSON. In the Peakboard Power BI control filter property you can use dynamic placeholders. Feel free to download the [attached pbmx](/assets/2024-09-07/PowerBISalesReport.pbmx) to find out more about the mentioned examples. In the first example we just make the country dynamic:

{% highlight json %}
[
{ "target": { "table": "financials", "column": "Country" },
"operator": "In", "values": ["#[CountryFilter]#"]}
]
{% endhighlight %}

![image](/assets/2024-09-07/030.png)

So to set and activate the filter during runtime we can just set the content of the variable and then call "Update Filters" to refresh the control.

![image](/assets/2024-09-07/040.png)

If things get more sophisticated just exchanging single values, we can make the shole JSON string dynamic instead of one single value.

![image](/assets/2024-09-07/050.png)

And then build the whole JSON string within our Building Blocks or LUA.

![image](/assets/2024-09-07/060.png)

## conclusion

Setting dynamic filters through JSON string is a perfect way to fully integrate Power BI dashboards in Peakboard applications dynamically. Building the right dynamic JSON string might by a challenge, especially because the Power BI filter engine won't give you any hint if the JSON is not correct or not undrstood by the engine.  

![image](/assets/2024-09-07/result.gif)

