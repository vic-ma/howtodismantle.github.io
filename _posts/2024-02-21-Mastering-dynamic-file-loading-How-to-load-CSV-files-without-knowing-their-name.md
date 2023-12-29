---
layout: post
title: Mastering dynamic file loading - How to load CSV files without knowing their names
date: 2023-03-01 12:00:00 +0200
tags: tutorial basics
image: /assets/2024-02-21/title.png
read_more_links:
  - name: Network file extension
    url: https://templates.peakboard.com/extensions/Network-Files/index
downloads:
  - name: ReadMultipleFilesFromUNCPath.pbmx
    url: /assets/2024-02-21/ReadMultipleFilesFromUNCPath.pbmx
---

Loading a CSV file with Peakboard is straightforward---no need to write an article about that. But what about when you don't know exactly which files to load, or if the data you're looking for is spread out across multiple files. This is exactly the problem we will be solving today.

## The scenario

Let's say that every couple of seconds, an air conditioner writes a log entry to a CSV log file in a shared directory. Each entry has a timestamp, temperature, and power consumption.

The tricky thing is, the AC starts a new log file every hour, and it gives it a dynamic name that contains the current date and time. So throughout the day, there will be multiple log files in the directory, like this:

![image](/assets/2024-02-21/010.png)

Here's what the contents of the files look like:

![image](/assets/2024-02-21/020.png)

Now, let's say we want to get a list of all log entries in the last 60 minutes. These entries will almost certainly span two log files. (The only time it wouldn't is if the time is exactly `XX:00:00`.)

So, we will need to read from two log files: the current log file and the last log file.

## The plan

Here are the steps we will follow:

1. List all the files in the shared directory. Sort them in descending order by name, so that the current log file is the first file, and the previous log file is the second file.
2. Load all log entries from the current log file.
3. Load all log entries from the previous log file.
4. Join the data from the two log files and sort the result in descending order by timestamp.

## Load the available files

For Peakboard to query all the files in a shared directory, we need to install the Network Files extension.

![image](/assets/2024-02-21/030.png)

The configuration for the network file list needs the credentials (domain, username, and password) to access the folder, as well as the actual UNC path. We trigger the data load to make sure it works.

![image](/assets/2024-02-21/040.png)

Now we need to make sure the file list is ordered correctly. So we add a data flow to the data source with only one step, which is the sorting of the list in descending order.

![image](/assets/2024-02-21/050.png)

## Setting up the data sources

We need two CSV data sources that look very similar. The first one, named `ACListCurrent`, uses the same credentials that we used for the network file list. The trick here is to not have a static file name, but rather use a single line LUA script. This script gets the file name from position 0 from the `ReorderFiles` data flow we created in the last step.

![image](/assets/2024-02-21/060.png)

We do the same thing for the `ACListLast` data source, except we point to position 1, which is the second entry of the file list.

![image](/assets/2024-02-21/070.png)

## Joining the data source

The next step to join the data sources from the last two steps.
We do this within a data flow below the ACListLast data source. The first step is to join the ACListLast with ACListCurrent. In the sense of a database join, it would be a so called "union join".

![image](/assets/2024-02-21/080.png)

After joining we need to sort the data by the time stamp column TS descending, to make sure the last entries are the top one.

![image](/assets/2024-02-21/090.png)

## A matter of the right order - the reload flow

To get the correct final results, we must make sure, that the steps outlined above are happening in the right order. For example it doesn't make sense to load the file before we know which file to load by querying the network file list. To ensure the correct order we use a so called Reload Data Flow. The data flows are not triggered independently but through the Reload flow. To do this, we set the "Reload State" to "Reload flow".

![image](/assets/2024-02-21/095.png)

After accessing the design of the reload flow (let's call it the "Master Flow"), we can make sure, that the file list is executed first and this in turn triggers the reload of the file reading data sources. We must understand, that the data flows below a data source are triggered automatically as part of the orginal data source. That's why only the data source but not the data flows are part of the master flow.

![image](/assets/2024-02-21/100.png)

## Result and conclusion

The last screenshots show the result of the process explained earlier. The first table on the upper left part shows the list of files sorted by date in descending order. Below this table you see the conents of the two files we selected (the current and the last before the current one). On the right side is the final resultset containing entries from both hours (starting from 17.00, the current and starting from 16.00 the previous)

![image](/assets/2024-02-21/110.png)