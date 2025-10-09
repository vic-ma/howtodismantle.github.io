---
layout: post
title: Hub Flows V - Condense, Archive, Optimize - Use Hub Flows to Pre-Aggregate and Archive High-Volume Transaction Data
date: 2023-03-01 00:00:00 +0000
tags: hubflows
image: /assets/2025-10-18/title.png
image_header: /assets/2025-10-18/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles about Hub Flows
    url: /category/hubflows
downloads:
  - name: ArchiveAndAggregate.pbfx
    url: /assets/2025-10-18/ArchiveAndAggregate.pbfx
---
This article part 5 of our [Hub Flows series](/category/hubflows). Today, we'll explain how to handle high-volume transaction data. Before continuing, make sure you understand the [basics of the Hub Flows](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html).

## Introduction

Many of the machines and sensors in a warehouse or factory floor produce large amounts of data, continuously. Usually, the most important data is the data from the last couple of minutes, hours, or days. This recent data is queried frequently, and must be delivered quickly.

Older data, however, should still be made available for a long-term analysis or in an aggregated form.
In this article we want to solve two tasks to handle this large aomunt of data:

1. We want to build a functionality that aggregates the data on a daily basis. After the day is over, the minimum, maximum and average temperature is stored in a seperate table for each day. So if someone needs this statistical data, it's not necessary anymore to aggregate the data from the original raw data. The temperature values are already pre-aggregated per day and so the access to this information doesn't need any computing power.

2. Let's assume a lot of other application are accessing the latest temperature data from the last couple of hours with a very high frequency. When we store the last months or even years in the same table. This process gets slower and slower over time. That's why build an archiving functionality. As soon as the data is older than 7 days, it's is copied from the actual trasaction table to an archive table. Using this arhcitecure no data is lost, but accessing the most needed data is still very fast because the table stays small.

 In our example we will handle data form a temperature sensor we already used in our [very first article](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html).

Our sample Hub list where the actual values are stored is called "TemperatureActual". Every couple of minutes the sensor generates and stores the current value in the column "Temperature" along with a time stamp in the column "TS".

![image](/assets/2025-10-18/010.png)

## Build the Data Aggregation Hub Flow

The first thing we need is a table to store the aggregated data in and name it "TemperatureDaily". We will need a column of the date and also for the minimum, maximum and average temperature on that day.

![image](/assets/2025-10-18/020.png)

In the Hub FLow project we set up a data source to access this table.

![image](/assets/2025-10-18/022.png)

For selecting the data to be aggregated we use the options to access the Hub Flows list through SQL. Below you see the SQL statement. So we do the actual aggregation already in the SQL statement. And we only select data before the current day to make sure, we don't write any aggregation before the day is over. And of cours we only aggregate the days which are not yet written to the "TemperatureDaily" table. The term "left(TS, 10)" is used to turn the time stamp into a day value without the time information.

{% highlight sql %}
select left(TS, 10) as Date, 
    min(Temperature) as MinTemp,
    max(Temperature) as MaxTemp, avg(Temperature) as AvTemp
from TemperatureActual where TS < FORMAT(GETDATE(), 'yyyy-MM-dd')
    and left(TS, 10) not in (Select date from TemperatureDaily)
group by left(TS, 10)
order by 1
{% endhighlight %}

![image](/assets/2025-10-18/024.png)

The next thing we need is a function that does the actual data transfer. We just loop over the data that is coming from the source and store each line in the new "TemperatureDaily" table. Usually it's just one line per day. But if the function is accidently not executed one day for whatever reasons, the next day the missing rows are also handled correctly.

![image](/assets/2025-10-18/026.png)

In the last step we put everything together and build the Hub FLow. First reload the aggrgation data source and then execute the function to store the output into the new table. As a trigger we use a sheduler and let the Flow automatically execute every night at 11PM.

![image](/assets/2025-10-18/028.png)

In case the destination table is completely empty, which it is the case when we set up this procedure. All the missing rows from the past days are created automatically. When it runs on daily basis only one row per day is written. The screenshot shows the data. We can see that January 10 was the first day the sensors has produced data so this will result in the first row of the aggregation table.

![image](/assets/2025-10-18/029.png)

## Archive

In our second use case, we really want to move data. The source table is the same "TemperatureActual" and the destination table is "TemperatureArchive". It has exactly the same columns "TS" and "Temperature". As already mentioned the main objective is to keep "TemperatureActual" nice and small to avoid any negative impact on production applications that rely on extremely fast table access. We will move all data older than 7 days from actual to archive.

Let's have a look at the data source. We need a data source that just points to our "TemperatureArchive" table, otherwise we're unable to store data into it later. 

For loading the data to be archived, we use a similiar SQL technique as with the first example. We just select all data that is older than 7 days and that is not yet in the archive table. As you see in the SQL statement we treat the TS column as some kind of primary key to check if the data is already transferred.

{% highlight sql %}
select * from TemperatureActual
where TS < FORMAT(GETDATE() - 7, 'yyyy-MM-dd')
    and TS not in (select TS from TemperatureArchive)
{% endhighlight %}

![image](/assets/2025-10-18/030.png)

Let's check the function that is doing the actual work. We just loop over each row to be archived and store in the archive. In the next step the original data row is deleted from the actual table

![image](/assets/2025-10-18/032.png)

The Hub Flow looks very similiar to the first one. We just execute the query to get the data to be archived and then call the function to store away the data and delete from the source table.

![image](/assets/2025-10-18/034.png)

## result and conclusion

In today's article we discussed two options to optimize tables: Pre-aggregation and archiving. These are most common use cases of that pattern and it's no problem to even use combination of both: Storing away the agregated data and then deleting the original data. In that case some information is lost, but if it's no need to keep it, it might by an option to do so.

