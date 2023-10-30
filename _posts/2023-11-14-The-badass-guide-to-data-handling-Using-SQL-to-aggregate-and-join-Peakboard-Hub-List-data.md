---
layout: post
title: The badass guide to data handling - Using SQL to aggregate and join Peakboard Hub List data
date: 2023-03-01 12:00:00 +0200
tags: sqlserver peakboardhub
image: /assets/2023-11-14/title.png
read_more_links:
  - name: SQL Joins
    url: https://www.w3schools.com/sql/sql_join.asp
  - name: Min and Max functions
    url: https://www.w3schools.com/sql/sql_min_max.asp
  - name: Learn the basics of Peakboard Hub Online
    url: /Peakboard-Hub-Online-An-introduction-for-complete-beginners.html
---

We already covered various topics around Peakboard Hub lists in other articles (please check out links at the bottom of this page). In today's article we discuss the option, to use SQL statements in combination with functions for aggregation and table join to access the Hub tables.
Why you want to do that when you can use data flows for data manipulation? The most obvious reason is to limit the network traffic. Let's say we have 10k+ data rows of sensor data and you want to visualize only the number of rows. Then it doesn't make sense to download 10k+ rows only to count them. Let the Hub do the counting and just download one row with the number in it. The same is true with table joins: Let's assume we have orders and order lines. The order has a date column, but you want to see the orderlines of the order at a specific date. Then you can download all orders and all orderlines, join them and throw away all order lines that do not belong to orders at a specific date. Or you can use a table join to solve this problem already on the source side without transporting to much data over the network. 

## Aggregate data by using aggregate functions

The basis of this example is a simple table of sensor data, to be more precise a list of temperature values. We have two columns: 'TS' for the date and time when the temperature was taken, 'temperature' for the actual temperature.

![image](/assets/2023-11-14/010.png)

Let's assume now we are not interested in all the detail data. The only thing we want to know (and present on our dashbaord later) is the maximum and the minimum temperature that was recorded today. Here's how the SQL statement looks like that is submitted to be processed by the hub:

{% highlight sql %}
select 
min(temperature) as MinTemperature, 
max(temperature) as MaxTemperature
from temperaturedata
where cast(TS as date) = cast(getdate() as date)
{% endhighlight %}

To use direct, plain SQL in the Peakboard Hub List data source, you need to activate the SQL mode by clicking on the button:

![image](/assets/2023-11-14/020.png)

The you can use the above statement and check your result on the right side. Now you have condensed thousands of rows to precisely two numbers you're interested in:

![image](/assets/2023-11-14/030.png)

## Joining hub lists

In our second example we have very simple relational connection between two tables: 'Products' contain the products we have on stock. Beside the product number there's also quantity of goods in the warehouse listed. Beside this table we have another one called ProductText. It contains a breif product description for each product numberin different languages (English, German and Chinese).

Here's the Product table:

![image](/assets/2023-11-14/040.png)

And the table for the product texts:

![image](/assets/2023-11-14/050.png)

What we want to have in our Peakboard application is one single table that contains our current stock quantities for each product PLUS the German description texts. Here we go with SQL:

{% highlight sql %}
select p.ProductNo, Quantity, Producttext
from
Products as p inner join ProductTexts as pt
on p.ProductNo = pt.ProductNo
where Language = 'DE'
{% endhighlight %}

and here is the result:

![image](/assets/2023-11-14/060.png)

## Conclusion

Preparing data already in the source before bringing them to the Peakboard application is a wise choice to increase performance and limit network bandwidth. Beside Peakboard Hub lists (as shown in this article) this works in almost any database or SQL based environment. we msut consider this option every time we design a new data source for a Peakboard project.

