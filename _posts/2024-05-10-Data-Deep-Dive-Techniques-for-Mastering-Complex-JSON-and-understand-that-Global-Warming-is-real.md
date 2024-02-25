---
layout: post
title: Data Deep Dive - Techniques for Mastering Complex JSON and understand that Global Warming is real
date: 2023-03-01 02:00:00 +0200
tags: api basics
image: /assets/2024-05-10/title.png
read_more_links:
  - name: Taming JSON - How to use JPath in Peakboard scripts
    url: /Taming-the-wild-JSon-How-to-use-JPath-in-Peakboard-scripts.html
  - name: Climate Reanalyzer
    url: https://climatereanalyzer.org
downloads:
  - name: ClimateTemperatureChart.pbmx
    url: /assets/2024-05-10/ClimateTemperatureChart.pbmx
---
Climate change and global warming are real, and anyone who digs into the numbers and facts is often shocked by how clear and obvious it is. We will discuss this in today's article. We will also learn a nice approach for processing JSON data that seems almost unmanageable at first.

The data we will use comes from a website called [Climate Reanalyzer](https://climatereanalyzer.org/), which is run by the University of Maine. It makes raw weather data---especially historic climate data---available to the public.  [Here](https://climatereanalyzer.org/clim/t2_daily/?dm_id=world), you can find some information about the daily surface temperature in the Northern Hemisphere, since 1940.

## The data

The actual data we want is available at this URL:

{% highlight url %}
https://climatereanalyzer.org/clim/t2_daily/json/era5_world_t2_day.json
{% endhighlight %}

The JSon data is organised in two large, nested arrays. The outer array contains one entry per year. Every year entry offers a minimum of metadata (like the actual year value) and also another array with temperatures values - one per day. Within the daily temperature array only the position determines the actualy data. At the first position with ordinal number 0 it's January 1st, the second one is January 2nd and so on.

![image](/assets/2024-05-10/010.png)

It is worth to mention that in the curent year the array still has 365 entries but the days of the future contain a 'nil' value.

![image](/assets/2024-05-10/020.png)

Beside each year the JSon also offers some service to pre-calculcate average temperatures over several years. The screenshot shows the average temperature of the years 1990 to 2020.

![image](/assets/2024-05-10/030.png)

## Build the data source

The first data source is just a regular JSon data source as shown in the screenshot. The data source offers the user to fill in a path to point to a certain area wihin the JSon file. As each year is one array entry and we start with 1940 with ordinal number 0, so the year 2024 is ordinal number 84. We might also use the button with 3 dots to let the Peakboard designer show a hierarchical outline of the data. Sometimes it can be tricky to find the right path. The actual output is the temperature array.

![image](/assets/2024-05-10/040.png)

For using the data in a chart we need to beautify it a little bit by using a data flow. The first step is to give the unknown column a better name: 'Temperature'. The second step is to add a column for the actual data. There's a small Lua script behind the column generation. It's the data January 1st plus the index number of the row to calculate the date. From the data we only take day and month, as the year is determined by the data source itself.

![image](/assets/2024-05-10/050.png)

The next step is straight forward, just turning the data type of the temperature into a number.

![image](/assets/2024-05-10/060.png)

And also filter out the 0 values. We don't need them anymore.

![image](/assets/2024-05-10/070.png)

## Building the chart

We end up with three data sources, one for the average of year 1990-2020, one for 2023 and one for 2024. The data flows are always the same. The only thing that changes is the path of the JSon data source.
To visualize the data we use a line chart with 3 series. Each of them pointing to a different source. If the data is perfecly prepared binding the chart to the data is straight forward.

![image](/assets/2024-05-10/050.png)

## result and conclusion

The last screenshot shows the result of the prject we built together. We learned two main points:

1. Even if the initial JSon looks pretty messy it's in most cases easy to get the data into a shape where it can be easily handled. Even if it looks tempting to bind it directly to a chart it's superimportant to clean up the data first: Give the column proper names, turn the values into proper data types, etc.... All that work pays off later.
2. Climate change and global warming is real. Everybody is encouraged to download the pbmx and play around with the data. The way the surface temperature is changing is so abvious and clear that there should be no discussions. The above mentioned website offers a wide variaty of other data to play around.

![image](/assets/2024-05-10/result.png)


