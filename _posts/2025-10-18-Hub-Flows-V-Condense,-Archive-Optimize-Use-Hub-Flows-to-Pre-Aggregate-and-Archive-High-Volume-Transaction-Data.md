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

Many of the machines and sensors in a warehouse or factory floor produce large amounts of data, continuously. And Peakboard apps often use this data to do different things, such as visualizing the data, providing data insights, or sending a notification when anomalous data is detected.

### Data pre-aggregation

However, applications often do not need or want the raw data from the machines. For example, let's say you have a temperature sensor in your warehouse. And let's say you have a Peakboard application that outputs the maximum, minimum, and average temperatures of the last 7 days.

But the temperature sensor generates new data every 6 minutes. In that case, it would be annoying if the Peakboard app had to get the raw data and aggregate it itself. And in the real world, you may have *multiple* Peakboard apps use the same data---and each one would need to independently aggregate the data. It would be better if we could provide *pre-aggregated data* for our Peakboard apps. 

So, our first goal is to build a Hub Flow that aggregates raw data, to make it easier for the apps that use the data.

### Data archival

But there's another problem: Our aggregated temperature table will eventually grow quite large, slowing down access speeds. And in the real world, you may have multiple apps querying the same data multiple times, making the effect even worse.

But we can't just delete the older data. We may need it for long-term analysis, so we need to keep the data.

So, our second goal is to build a Hub Flow that deletes and archives any data older than 7 days, from our aggregated temperature table. This keeps the aggregated table small, so that queries to it remain fast. But, we keep all that deleted data in a separate archival table, so it is still accessible.

### Temperature sensor

For our example, we will use [data from a temperature sensor](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html).

The raw temperature data is stored in a Hub list called `TemperatureActual`. Every 6 minutes, the sensor adds a new row to the table, with the current temperature in the `Temperature` column, and the timestamp in the `TS` column.

![image](/assets/2025-10-18/010.png)

## Build the data aggregation Hub Flow

Now, let's build the Hub Flow. Here's an overview of how the finished Hub Flow works:
1. The physical temperature sensor writes new data to the `TemperatureActual` Hub list, every 6 minutes.
1. The Hub Flow's `TemperatureForAggregation` data source reads `TemperatureActual` and aggregates the data.
1. The Hub Flow's writes the data from `TemperatureForAggregation` to `TemperatureDaily` Hub list.

### Create the aggregate data Hub list

First, we create the [Peakboard Hub list](/Peakboard-Hub-Online-Using-lists-to-store-sensor-data.html) that will store the aggregated data. We name it `TemperatureDaily`. We add the following columns:
* The date
* The minimum temperature
* The maximum temperature
* The average temperature

![image](/assets/2025-10-18/020.png)

### Create the data source for the aggregate data Hub list

Next, in our Hub Flow, we add a data source to access this table:

![image](/assets/2025-10-18/022.png)

### Create the data source for aggregate data

Next, we create the data source that generates the aggregate data, by using a SQL statement. Here's the SQL statement:


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

For selecting the data to be aggregated we use the options to access the Hub Flows list through SQL. Below you see the SQL statement. So we do the actual aggregation already in the SQL statement. And we only select data before the current day to make sure, we don't write any aggregation before the day is over. And of cours we only aggregate the days which are not yet written to the "TemperatureDaily" table. The term "left(TS, 10)" is used to turn the time stamp into a day value without the time information.

### Create the function that writes the aggregate data

The next thing we need is a function that does the actual data transfer. We just loop over the data that is coming from the source and store each line in the new "TemperatureDaily" table. Usually it's just one line per day. But if the function is accidently not executed one day for whatever reasons, the next day the missing rows are also handled correctly.

![image](/assets/2025-10-18/026.png)

In the last step we put everything together and build the Hub FLow. First reload the aggrgation data source and then execute the function to store the output into the new table. As a trigger we use a sheduler and let the Flow automatically execute every night at 11PM.

![image](/assets/2025-10-18/028.png)

In case the destination table is completely empty, which it is the case when we set up this procedure. All the missing rows from the past days are created automatically. When it runs on daily basis only one row per day is written. The screenshot shows the data. We can see that January 10 was the first day the sensors has produced data so this will result in the first row of the aggregation table.

![image](/assets/2025-10-18/029.png)

## Build the data archival Hub Flow

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

