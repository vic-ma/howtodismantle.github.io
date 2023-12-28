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

Loading a CSV file with Peakboard is quite straight forward - no need to write an article about that. But what to do when you don't know exactly which files to load and what about if the information you're looking for is distributed over multiple files.
Here's the situation we start with: An air condition appliance is writing out a log file entry every couple of second to a shared directory. Within these CSV log files there are entries for the time stamp, temperature and power consumption. The tricky thing is, that the AC starts a new logfile every full hour and gives it a dynamic name that contains the current date and time. So thoughout the day there will be multiple log files in the directory:

![image](/assets/2024-02-21/010.png)

Here's how the content of the file looks like:

![image](/assets/2024-02-21/020.png)

## What is the logic of what we are building?

What we want to have at the end is a list of all logfile entries for the last 60 minutes. So as long as there is not a whole hour in one file (which literally only happens for one second per hour) we need to read the current file and the last file before the current and join them together. So here are the logical steps:

1. List all files from the directory and sort them in descending order so that the current file is on number 1 and the last one before the current one is on number 2
2. Load all log entries from the current file
3. Load all log entries from the last file before the current file
4. Join the data from 2 und 3 together and sort the result in descending time stamp order

## Loading the available files

For querying all file from a directory, we need to install the network file extension.

![image](/assets/2024-02-21/030.png)

The configuration of the network file list needs the credentials (domain, user name, password) to access the folder, as well as the actual UNC path. We can easily trigger the data load and see if it works.

![image](/assets/2024-02-21/040.png)

Now we need to make sure the file list is ordered correctly. So we add a data flow to the data source with only one step, which is the sorting of the list in descending order.

![image](/assets/2024-02-21/050.png)

## Setting up the data sources

We need two CSV data source that look very similiar. The first one named ACListCurrent uses the same credentials we already use for the network file list. The trick here is not to have a static file name, but use a single line LUA script to get the file name from the position number 0 from ReorderFiles data flow we created in the last step.

![image](/assets/2024-02-21/060.png)

We do the same for the ACListLast data source source, but here we point to ordinal number 1 (whichis the second entry of the file list).

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