---
layout: post
title: How to handle historian machine state data in SQL queries and turn it into useful information
date: 2023-05-17 12:00:00 +0200
tags: essentials sqlserver
image: /assets/2023-05-23/020.png
---
 In this [article](2023-05-01-Best-Practice-Store-machine-states-in-SQL-Server-and-build-data-historian.md) we learned how to store historical events in a SQL Server database. Every time a state changes, a new row is created with a timestamp and the name of the new state. As you can see in the sample data, there are two drilling machines switching between RUN and STOP throughout a regular working day.

![image](/assets/2023-05-23/010.png)

They way the information is stored is of course unsuitable for end users and relatively hard to be used for analysis purpose. In this article we will learn a best practise how to handle this kind of data easily by using intelligent SQL commands.

The first thing we want to do is to transform the original data into a table where each state has a start date and an end date because then it's much easier to do calculcations. To achieve this we use the SQL command _LEAD_. If you have never heard of that, please check out the documentation provided my [Microsoft](https://learn.microsoft.com/en-us/sql/t-sql/functions/lead-transact-sql?view=sql-server-ver16).
The _LEAD_ command as shown in the sample says, for each row, find the next row with a higher timestamp (_Order by TS_), but not just any next row, but the next state row of the same machine. That's where the _PARTITION BY MachineName_ comes into play.

{% highlight sql %}
SELECT MachineName, State, TS as TimeStart,   
	LEAD (TS) OVER (PARTITION BY MachineName ORDER BY TS) AS TimeEnd
FROM MachineStateHistory
{% endhighlight %}

This is how the resultset looks like:

![image](/assets/2023-05-23/020.png)

Later on, we will probably need the time difference between the start and the end data of each state, preferebly in minutes. It would no problem to calculcate this in Peakboard, however in this case it's a bit more elegant to let the SQL server just do the work by using the _DataDiff_ command. Don't forget to check the [official documentation](https://learn.microsoft.com/en-us/sql/t-sql/functions/datediff-transact-sql?view=sql-server-ver16) if you're not familiar.
There are several ways on how to apply the _DataDiff_ together with the _LEAD_ command. The easiest way is to wrap the _LEAD_-statement into a _WITH_ statement and the build a second _SELECT_ around that resultset.

{% highlight sql %}
WITH results AS
(
	SELECT MachineName, State, TS as TimeStart,   
		   LEAD (TS) OVER (PARTITION BY MachineName ORDER BY TS) 
                AS TimeEnd
	FROM MachineStateHistory
)
SELECT *, DATEDIFF(mi, TimeStart, TimeEnd) as TimeSpanInMinutes
   FROM results;
{% endhighlight %}

And here's how the resultset looks like in Peakboard Designer:

![image](/assets/2023-05-23/030.png)

On basis of that resultset it's supereasy to calculate all related values just by using a dataflow. The sample shows how to aggregate the sum of all time span values and group it by machine and state. Then filter away anything other than State _RUN_, then filter away anything other then Machine _Drilling01_. And we end up with one single row repesenting the time that _Drilling01_ machine has run in total.

![image](/assets/2023-05-23/040.gif)

From here it's easy to finally bring it home. The aggregated number is just displayed in a simple tile. Mission accomplished.

![image](/assets/2023-05-23/050.png)

Please keep one important learning in mind: Although Peakboard offers multiple ways of manipuating data it's usually a wise choice to already prepare the data for future use when creating the SQL statement. It can save lots of time later on and makes the whole data logic much easier to understand and maintain.

