---
layout: post
title: Snow Business Like Data Business - Peakboard Meets Snowflake
date: 2023-03-01 03:00:00 +0200
tags: bi 
image: /assets/2025-11-19/title.png
image_landscape: /assets/2025-11-19/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Download the Snowflake ODBC driver
    url: https://docs.snowflake.com/en/developer-guide/odbc/odbc-download
  - name: ODBC setup guide
    url: /SAP-Hana-Meets-Peakboard-Mastering-ODBC-Integration-Step-by-Step.html
downloads:
  - name: Snowflake.pbmx
    url: /assets/2025-11-19/Snowflake.pbmx
---

Over the past few years, [Snowflake](https://www.snowflake.com/) has become one
of the world's leading vendors for cloud-based data warehousing and BI backends.
Snowflake combines flexibility, governance, and impressive elasticity---all in
one platform---and all while remaining remarkably approachable for new teams.

In this article, we'll explain how to connect a Peakboard application to a
Snowflake database, by using an ODBC driver. (However, there
may be other options for connecting to Snowflake.)

Snowflake offers a JSON-based REST API and a full range of language-specific
connectors. Check out their [REST API
docs](https://docs.snowflake.com/en/developer-guide/snowflake-rest-api/snowflake-rest-api)
to learn more, and to compare the different interfaces and find the one that's most suitable for your application.

## Set up your environment

Our first step is to download the [ODBC driver for Windows (64
Bit)](https://docs.snowflake.com/en/developer-guide/odbc/odbc-download).
Once it's downloaded, we install the driver on **two machines**:
* The machine with Peakboard Designer. This is the machine where we'll build the Peakboard app on.
* The machine where the Peakboard app will run on. This is either a Peakboard Box or Peakboard BYOD instance.

We explained the [ODBC setup process on Peakboard](/SAP-Hana-Meets-Peakboard-Mastering-ODBC-Integration-Step-by-Step.html) in a previous article. Check it out for the full step-by-step guide.

During installation, make sure to select the correct architecture (either 32 or
64 bit) for the Peakboard runtime. And make sure to keep the driver updated---Snowflake regularly releases new drivers with security patches and
performance improvements.

## Set up the Snowflake database

For our example app, we'll connect to a very simple table in our Snowflake
database. We created the table manually, in the Snowflake backend. The table
contains time series data from an air conditioning system. It has the following
columns:
* `TS`, the timestamp.
* `TEMPERATURE`, the temperature.
* `COOLING`, a boolean for whether or not the A/C is actively cooling.

This app idea is based on an OPC UA article we did 2 years ago, about [how to
connect to an A/C via OPC
UA](https://how-to-dismantle-a-peakboard-box.com/OPC-UA-Basics-Calling-functions-in-OPC-UA-and-switch-the-AC-off.html).

For production workloads you would typically create the table through an
automated deployment script, apply proper clustering keys, and enable time
travel retention for safer rollbacks and audits.

![image](/assets/2025-11-19/snowflake-temperature-table-sample.png)

## Create the connection string

We need a connection string to connect to our Snowflake database. Here's the
connection string that we use for our `DISMANTLEDB` database:

{% highlight text %}
Driver=SnowflakeDSIIDriver; Server=VAAHZZD-XF03787.snowflakecomputing.com; UID=zuteilungsreif; PWD=supersecret; Warehouse=COMPUTE_WH; Database=DISMANTLEDB; Schema=PUBLIC; Role=SYSADMIN;
{% endhighlight %}

You can use it as a template, and replace the parameters with your own server,
UID, etc. To learn more, check out the official [ODBC parameters
documentation](https://docs.snowflake.com/en/developer-guide/odbc/odbc-parameters).
The document also explains how to add optional settings like
`Authenticator=externalbrowser` or `Tracing=6`, for troubleshooting purposes.
So keep the docs open while you're configuring the driver and documenting your setup!

## Create the ODBC data source

Now, the hard part is over. Once we have the connection string, we can create the ODBC data source in Peakboard Designer:

![image](/assets/2025-11-19/snowflake-odbc-connection-setup.png)

First, we enter our connection string in the **Connection string** box. Then, we enter a SQL statement to specify the data we want from our Snowflake database. Here's what we use:
```sql
select * from DISMANTLEDB.PUBLIC.ACLOG
```

It says to get all the columns from the `ACLOG` table in our `DISMANTLEDB`
database.

When you write your SQL statement, make sure you use the correct namespace for the tables you want to access. We already provided a default schema in the connection string, so we actually don't need to use the fully qualified name in our SQL statement---as long as we don't need to access any tables outside of the default namespace:
```sql
select * from ACLOG
```

If you need to join other schemas, then prefix them with the database name or
adjust the default schema in the connection string, and double-check that the
assigned Snowflake role has the necessary privileges.

## Write data to a Snowflake database

Uploading large amounts of data efficiently is one of the USPs of Snowflake's
database product. Usually, data is uploaded as a CSV file to some data store
(like Azure Blob Storage). Then, the CSV file is uploaded from the data store to
the Snowflake database. This process is usually triggered by [REST API
calls](https://docs.snowflake.com/en/developer-guide/snowflake-rest-api/snowflake-rest-api).

For our example, however, we'll do things in a different way. We'll just use a
SQL `INSERT` statement, and rely on Snowflake's automatic transaction handling
to keep the data consistent. This is less efficient, but it's easier to do, and
keeps the process manageable for smaller teams. And it's very reliable if the
amount of data being inserted is not huge. 

Here's what an example of an `INSERT` statement might look like:

{% highlight sql %}
INSERT INTO DISMANTLEDB.PUBLIC.ACLOG (TS, Temperature, Cooling)
VALUES    ('2025-07-03 15:15:01', 27.5, False);
{% endhighlight %}

For Peakboard Designer, we use placeholders to build the SQL string:
{% highlight sql %}
INSERT INTO DISMANTLEDB.PUBLIC.ACLOG (TS, Temperature, Cooling)
VALUES    ('#[TS]#', #[Temperature]#, #[Cooling]#);
{% endhighlight %}

The follow screenshot shows how to apply the statement and use a placeholder
Building Block to fill the placeholders with actual values. You
can further enhance the workflow by adding pre-validation logic to check for
duplicate timestamps---or by wrapping the execution in a simple retry block to
cope with transient connectivity issues (especially when the Peakboard Box is
deployed in networks with fluctuating bandwidth).

![image](/assets/2025-11-19/snowflake-sql-insert-building-block.png)

## Result and conclusion

In this article, we looked at how easy it is to use ODBC to access a Snowflake
database for both reading and writing. We noted that as long as the throughput
is not unreasonably large, it's much easier to use SQL `INSERT` statements,
rather than the traditional method of uploading through a file-based interface.
Remember to secure the credentials in the connection string, monitor warehouse
usage to avoid unexpected costs, and leverage Snowflake roles to restrict access
to only the tables required by the Peakboard application.