---
layout: post
title: Snow Business Like Data Business - Peakboard Meets Snowflake
date: 2023-03-01 03:00:00 +0200
tags: bi 
image: /assets/2025-11-19/title.png
image_landscape: /assets/2025-11-19/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Download Snowflake ODBC driver
    url: https://docs.snowflake.com/en/developer-guide/odbc/odbc-download
  - name: SAP Hana Meets Peakboard - Mastering ODBC Integration Step-by-Step
    url: /SAP-Hana-Meets-Peakboard-Mastering-ODBC-Integration-Step-by-Step.html
downloads:
  - name: Snowflake.pbmx
    url: /assets/2025-11-19/Snowflake.pbmx
---
Over the last years Snwoflake became one of the leading vendors for cloud based datewarehousing and BI backends.
In this week's article we will discuss a best practice way to connect a Peakboard application to a Snowflake database. We will use and ODBC driver for that, however there might by also other options to connect. Snowflake offers and JSON based REST API. We can check [this site](https://docs.snowflake.com/en/developer-guide/snowflake-rest-api/snowflake-rest-api) to learn more about that.

## Setting up the environment

The ODBC driver for WIndows (64 Bit) can be downloaded [here](https://docs.snowflake.com/en/developer-guide/odbc/odbc-download). It is very important to understand that the driver must be installed on the local machine where the Peakboard designer is running, but also on the Peakboard box or the Peakboard BYOD instance. We already discussed the ODBC setup on Peakboard as part of the [article about using ODBC to access SAP Hana databases](/SAP-Hana-Meets-Peakboard-Mastering-ODBC-Integration-Step-by-Step.html).

In our sample we will use a very simple table that we manually created in the Snowflake backend. The table is used to store temperature values from an air conditioning system along with timestamp and a boolean value to indicate of the A/C is running at teh moment. It's inspired by the OPC UA article we published 2 years ago about [how to connect to an A/C via OPC UA](https://how-to-dismantle-a-peakboard-box.com/OPC-UA-Basics-Calling-functions-in-OPC-UA-and-switch-the-AC-off.html).  

![image](/assets/2025-11-19/010.png)

## Using ODBC to connect to snowflake

We will need a connection string to connect to Snowflake. Here's the connection string related to our Dismnantle demo DB. It can be used as template to connect to the reader's own Snowflake database. TO learn about the details of each parameter we can refer to the [documentation](https://docs.snowflake.com/en/developer-guide/odbc/odbc-parameters).

{% highlight text %}
Driver=SnowflakeDSIIDriver; Server=VAAHZZD-XF03787.snowflakecomputing.com; UID=zuteilungsreif; PWD=supersecret; Warehouse=COMPUTE_WH; Database=DISMANTLEDB; Schema=PUBLIC; Role=SYSADMIN;
{% endhighlight %}

Building the connection string was the hardest part. After we solved that we can use it right away through the ODBC data source in Peakboard designer. The SQL statement we use is a common statement without any voodoo. A typical source of error is using the correct namespace to indicate the tables to access. We already provided a default schema in the connection, so actually we could skip the full qualified name within the SQL statement as long as we don't want to read "outside" the defualt namepsace. So our SQL statement is just `select * from DISMANTLEDB.PUBLIC.ACLOG` or `select * from ACLOG`. 

![image](/assets/2025-11-19/020.png)

## writing back to Snowflake

Uploading huge amounts of data very efficiently is one of the USPs of any Snowflake database. This is usally done through CSV files. These are uploaded to a storage (e.g. an Azure storage blob) and then taken from there to upload it to the database. This process is usually triggerd through JSON Rest calls (see documention above for details). In our sample we will go a different way. It's less efficient but easy to use and very reliable if the amount of data to insert is not endlessly huge. We just use an SQL Insert statement:

{% highlight sql %}
INSERT INTO DISMANTLEDB.PUBLIC.ACLOG (TS, Temperature, Cooling)VALUES    ('2025-07-03 15:15:01', 27.5, False);
{% endhighlight %}

To make it easier to read within Peakoard we ust use place holders to build the SQL string:

{% highlight sql %}
INSERT INTO DISMANTLEDB.PUBLIC.ACLOG (TS, Temperature, Cooling)VALUES    ('#[TS]#', #[Temperature]#, #[Cooling]#);
{% endhighlight %}

The screenshot shows how to apply the statement and use a placeholder Building Block to fill the dynamic parts of the statements with real values.

![image](/assets/2025-11-19/030.png)

## result and conclusion

This short article shows how easy it is to use ODBC to access Snowflake for both reading and writing. As long as the throughput is acceptable it's much easier to use SQL instead of the trdional way of uploading though a file based interface.
 