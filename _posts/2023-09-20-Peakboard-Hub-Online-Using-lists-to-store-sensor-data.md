---
layout: post
title: Peakboard Hub Online - Using lists to store sensor data
date: 2023-03-01 12:00:00 +0200
tags: peakboardhub opcuamqtt
image: /assets/2023-09-20/title.png
read_more_links:
  - name: Learn the basics of Peakboard Hub Online
    url: /Peakboard-Hub-Online-An-introduction-for-complete-beginners.html
  - name: Learn the basics of reading from an OPC UA server in Peakboard
    url: /OPC-UA-Basics-Getting-started-with-a-public-OPC-UA-server.html
downoads:
  - name: AirConditionerList.pbmx
    url: /assets/2023-09-20/AirConditionerList.pbmx
---

In this article, we will learn how to write sensor data to a list in [Peakboard Hub Online](/Peakboard-Hub-Online-An-introduction-for-complete-beginners.html) (PBHO). We will also learn how to read from a list. Finally, we will learn how to use a SQL command when reading from a list, in order to aggregate data.

What sensor data will we use? We will use the data from an air conditioner, which comes from a public OPC UA server. See [this article](/OPC-UA-Basics-Getting-started-with-a-public-OPC-UA-server.html) to learn the basics of reading from an OPC UA server in Peakboard.

## Create a data source for our sensor data

In Peakboard Designer, we add a new OPC UA data source for our sensor data, called `AirConditionerData`.

We subscribe to these three nodes:
* `Temperature`, the current ambient temperature
* `PowerConsumption`, the current power consumption
* `Quality`, the current air quality

Finally, we change the polling rate to 5 seconds, to slow it down a bit.

![image](/assets/2023-09-20/010.png)


## Create a new list in PBHO

In PBHO, we create an empty list. We will write our sensor data to this list.

We add columns that correspond to the nodes of our sensor data. For our `PowerConsumption` node, we will use a boolean that says if the air conditioner is actively cooling or not.

![image](/assets/2023-09-20/020.png)


## Create a data source for our PBHO list

In Peakboard Designer, we add a new PBHO list data source called `AirConditionerLogs`. Our Peakboard Box will use this data source to read from our PBHO list.

![image](/assets/2023-09-20/030.png)

Here's how we set up this data source:

1. We click *Load lists* to get the lists that are available in PBHO.
2. We select the empty list that we created in the previous section.
3. We select `ID` as the column we sort our data by. Each entry in the list is automatically assigned an `ID`, which is just an auto-increment counter.
4. We deselect *Ascending order*, in order to sort our data in descending order. This sorts the data from highest `ID` to lowest, meaning the newest entries will show up first.
5. We limit the data we read to the 10 newest rows, because our table control won't show more than 10 rows, and we don't want to download the entire PBHO list, which can grow quite large.
6. We click *Load data* in the Preview window, to load our empty PBHO list.

Because we will eventually reload this list with a script, we can disable reloading. But this is not essential and does not make much of a difference.

![image](/assets/2023-09-20/040.png)


## Create a script that writes to our PBHO list

We have a data source for both our sensor data and our PBHO list. Now we can create a script that writes the sensor data to the PBHO list. Each time new sensor data comes in, this script will write that data to the PBHO list.

First, we add a new refreshed script to `AirConditionerData`. This script will execute each time a data source refreshes (i.e. gets new data).

![image](/assets/2023-09-20/050.png)

Here is the script that we create:

![image](/assets/2023-09-20/060.png)

The root block adds a row to the end of the PBHO list. It come from *FUNCTIONS* > *Publish to external systems* > *Peakboard Hub* > *Add row at end*.

We fill the columns of our row with data from `AirConditionerData`. We get our `IsCooling` column by checking if the `PowerConsumption` is zero or not.

Finally, we reload our PBHO list, so that the control in our dashboard updates.

We will also uncheck the *Execute only if data changed* box. That's because we want to record all sensor data, even if it does not change from the last one.

![image](/assets/2023-09-20/070.png)


## Add tables for visualization

Now, we add two table controls: one for `AirConditionerData`, and one for `AirConditionerLogs`.

We can see that as new data comes in, it gets written to the PBHO list, and we read the updated PBHO list.

![image](/assets/2023-09-20/080.gif)


## Select from PBHO list with SQL

### The problem

Let's say we want to do some data aggregation on our PBHO list: we want to get the minimum, maximum, and average temperature recorded by the air conditioner. And we want to calculate this for all the data in the PBHO list, not just the last 10 rows. How can we do this?

One way is to remove the row limit we had in `AirConditionerLogs`. That way, the entire data set would be downloaded. We could then create a script that aggregates the data.

But that requires downloading the entire PBHO list, which can be quite large. It also requires the Peakboard Box to do the calculation on that large data set. Neither of these things are too desirable.

### The Solution

Instead, let's have PBHO do the calculation for us, and just give us the answers. That way, our Peakboard Box only downloads three numbers, and it doesn't have to do any of the calculations itself.

So, we create a new PBHO list data source called `AirConditionerLogsAnalysis`. We click *Load lists*, like before. But this time, we will click *Select with SQL* and enter the following SQL SELECT statement:

{% highlight sql %}
SELECT ROUND(AVG(Temperature), 2) AS AvgTemp, MAX(Temperature) AS MaxTemp, MIN(Temperature) AS MinTemp FROM AirConditionerLogs
{% endhighlight %}

The table name is the name of our PBHO list: `AirConditionerLogs`.

![image](/assets/2023-09-20/090.gif)

We also need to add a refresh block that refreshes `AirConditionerLogsAnalysis` to the script we created before.

Finally, we will add a table control to display `AirConditionerLogsAnalysis`.

Here's what the finished product looks like:

![image](/assets/2023-09-20/100.gif)
