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

In Peakboard Designer, we add a new OPC UA data source for our sensor data.

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

In Peakboard Designer, we add a new PBHO list data source. Our Peakboard Box will use this data source to read from our PBHO list.

![image](/assets/2023-09-20/030.png)

Here's how we set up this data source:

1. We click *Load lists* to get the lists that are available in PBHO.
2. We select the empty list that we created in the previous section.
3. We select `ID` as the column we sort our data by. Each entry in the list is automatically assigned an `ID`, which is just an auto-increment counter.
4. We deselect *Ascending order*, in order to sort our data in descending order. This sorts the data from highest `ID` to lowest, meaning the newest entries will show up first.
5. We click *Load data* in the Preview window, to load our empty PBHO list.

Because we will eventually reload this list with a script, we can disable reloading. But this is not essential and does not make much of a difference.

![image](/assets/2023-09-20/040.png)


## Create a script that writes to our PBHO list

We have a data source for both our sensor data and our PBHO list. Now we can create a script that writes the sensor data to the PBHO list. Each time new sensor data comes in, this script will write that data to the PBHO list.

First, we add a new refreshed script to our OPC UA data source. A refreshed script executes each time a data source refreshes (i.e. gets new data).

![image](/assets/2023-09-20/050.png)

Here is the script that we create:

![image](/assets/2023-09-20/060.png)

The root block adds a row to the end of the PBHO list. It come from *FUNCTIONS* > *Publish to external systems* > *Peakboard Hub* > *Add row at end*.

We fill the columns of our row with data from our OPC UA data source. We get our `IsCooling` column by checking if the `PowerConsumption` is zero or not.

Finally, we reload our PBHO list, so that the control in our dashboard updates.

We will also uncheck the *Execute only if data changed* box. That's because we want to record all sensor data, even if it does not change from the last one.

![image](/assets/2023-09-20/060.png)


re-add refresh script for analysis
