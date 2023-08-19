---
layout: post
title: Peakboard Hub Online - Use lists to store sensor data
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

In this article, we will learn how to write sensor data to a list in [Peakboard Hub Online](/Peakboard-Hub-Online-An-introduction-for-complete-beginners.html) (PBHO). We will also learn how to read from a list. Finally, we will learn how to use a SQL command when reading from a list, in order to aggregate the data.

For our sensor data, we will use the air conditioner data from a public OPC UA server. See [this article](/OPC-UA-Basics-Getting-started-with-a-public-OPC-UA-server.html) to learn more.


## Create a new list in PBHO

In PBHO, we create an empty list called `AirConditionerLogs`. We will write our sensor data to this list.

We add three columns, which correspond to the data we will get from the air conditioner:

* `Temperature`  (number), the ambient temperature.
* `IsCooling` (boolean), whether the air conditioner is actively cooling or not.
* `Quality` (string), the air quality.

![image](/assets/2023-09-20/020.png)


## Create a data source for our sensor data

In Peakboard Designer, we add a new OPC UA data source for our sensor data. We call it `AirConditionerData`.

We subscribe to these three nodes:

* `Temperature`, the ambient temperature.
* `PowerConsumption`, the power consumption. We will use this to get the `IsCooling` column in our PBHO list, by checking if `PowerConsumpition` is 0.
* `Quality`, the air quality.

Finally, we change the polling rate to 5 seconds, to slow it down a bit.

![image](/assets/2023-09-20/010.png)

## Create a data source for our PBHO list

In Peakboard Designer, we add a new PBHO list data source. We call it `AirConditionerLogs`. Our Peakboard Box will use this data source to read from our PBHO list.

![image](/assets/2023-09-20/030.png)

Here's how we set up this data source:

1. We set the reload state to *On Startup*.  Later on, we will have a script handle the periodic reloading.
2. We click *Load lists* to load all the lists that are available in PBHO.
3. We select `AirConditionerLogs`.
4. We sort this data source by `ID`, in descending order. This gets us the newest rows first.
5. We set 5 as the maximum number of rows to fetch, since we'll only display 5 at a time in our table control.
6. We press *Load data* to see a preview of PBHO list.

![image](/assets/2023-09-20/040.png)


## Create a script that writes to our PBHO list

We have a data source for both our sensor data and our PBHO list. Now we can create a script that writes the sensor data to our PBHO list.

First, we create a new refreshed script for `AirConditionerData`. This script will automatically execute each time `AirConditionerData` gets new data from the OPC UA server.

![image](/assets/2023-09-20/050.png)

Here is the script that we create:

![image](/assets/2023-09-20/060.png)

The main building block adds a row to the end of our PBHO list. It come from *FUNCTIONS* > *Publish to external systems* > *Peakboard Hub* > *Add row at end*.

Remember that this script executes each time new sensor data comes in. So, this script says: "Each time new sensor data comes in, add a row to the end of our PBHO list."

We will also reload the `AirConditionerLogs` data source at the end, so that our table control will be able to see the changes.

We will also uncheck the *Execute only if data changed* box. That's because we want to record all sensor data, regardless of if it changes from the last sensor data we got.

![image](/assets/2023-09-20/070.png)

{% comment %}
## Add tables for visualization

Now, we add two table controls: one for `AirConditionerData`, and one for `AirConditionerLogs`.

We can see that as new data comes in, it gets written to our PBHO list, and we read the updated PBHO list.

![image](/assets/2023-09-20/080.gif)
{% endcomment %}

## Select from PBHO list with SQL

### The problem

Let's say we want to do some data aggregation on our PBHO list: we want to get the minimum, maximum, and average temperature recorded by the air conditioner. And we want to calculate this for all the data in our PBHO list, not just the last 10 rows. How can we do this?

One way is to remove the row limit we had in `AirConditionerLogs`. That way, the entire data set would be downloaded. We could then create a dataflow to aggregate the data.

But that requires downloading the entire PBHO list, which can be quite large. It also requires the Peakboard Box to do the calculation on that large data set. Neither of these things are too desirable.

**Maybe mention non-aggregation use cases

### The Solution

Instead, let's have PBHO do the calculation for us, and just give us the answers. That way, our Peakboard Box only downloads three numbers, and it doesn't have to do any of the calculations itself.

So, we create a new PBHO list data source called `AirConditionerLogsAnalysis`. We click *Load lists*, like before. But this time, we will click *Select with SQL* and enter the following SQL SELECT statement:

{% highlight sql %}
SELECT ROUND(AVG(Temperature), 2) AS AvgTemp, MAX(Temperature) AS MaxTemp, MIN(Temperature) AS MinTemp FROM AirConditionerLogs
{% endhighlight %}

The table name is the name of our PBHO list: `AirConditionerLogs`.

![image](/assets/2023-09-20/090.png)

We also need to add a refresh block that refreshes `AirConditionerLogsAnalysis` to the script we created before.

Finally, we will add a table control to display `AirConditionerLogsAnalysis`.

Here's what the finished product looks like:

![image](/assets/2023-09-20/100.png)
