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
downloads:
  - name: HUBSQLHacks.pbmx
    url: /assets/2023-11-14/HUBSQLHacks.pbmx
---

We've already covered various topics related to Peakboard Hub lists in other articles (please check out the links at the bottom of this page). In today's article, we will learn how to use SQL statements in combination with functions for aggregation and table join, in order to access the Peakboard Hub tables.

## Why would you want to use aggregation / joins when you can use data flows for data manipulation? 

The most obvious reason is to limit the network traffic. Let's say you have 10k+ rows of sensor data, and you just want to visualize the number of rows. Then it doesn't make sense to download all 10k+ rows just to count them. Instead, you can let the Hub do the counting, and just download one row, which contains the result.

The same is true with table joins: Let's assume you have orders and order lines. The order has a date column. But you want to see the order lines of the order on a specific date. Then, you can download all orders and all order lines, join them, and throw away all order lines that do not belong to orders on your desired date. Or, you can use a table join to solve this problem on the source side, without transferring too much data over the network. 

## Aggregate data by using aggregate functions

Let's take a look at an example of how we can aggregate data by using aggregate functions.

The basis of this example is a simple table of sensor data. Specifically, it's a list of temperature values. We have two columns:

* **TS** - the date and time when the temperature was taken
* **Temperature** - the actual temperature

![image](/assets/2023-11-14/010.png)

Let's assume we are not interested in all the data. The only thing we want to know (and present on our dashboard later) is the maximum and the minimum temperature that was recorded today. To do that, we can give this SQL statement to the Hub:

{% highlight sql %}
select 
min(temperature) as MinTemperature, 
max(temperature) as MaxTemperature
from temperaturedata
where cast(TS as date) = cast(getdate() as date)
{% endhighlight %}

To use direct, plain SQL in the Peakboard Hub List data source, you need to activate the SQL mode by clicking on the "Select with SQL" button:

![image](/assets/2023-11-14/020.png)

Then, you can use the above statement and check your result on the right side. Now, you have condensed thousands of rows down to the two numbers you're interested in:

![image](/assets/2023-11-14/030.png)

## Joining Peakboard Hub lists

Now, let's take a look at how we can join two Peakboard Hub lists.

In this example, we have a very simple relational connection between two tables:

* **"Products"** contains the products we have in stock. It has a column for the product number, and a column for the quantity of that product in the warehouse. 

* **"ProductTexts"** contains a brief product description for each product number, in different languages (English, German, and Chinese).

Here's the "Products" table:

![image](/assets/2023-11-14/040.png)

And here's the "ProductTexts" table:

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

