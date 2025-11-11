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

Over the last few years, [Snowflake](https://www.snowflake.com/) has become one
of the world's leading vendors for cloud-based data warehousing and BI backends.
Snowflake combines flexibility, governance, and impressive elasticity---all in
one platform, and while remaining remarkably approachable for new teams.

In this article, we'll discuss the proper way to connect a
Peakboard application to a Snowflake database. We'll use an ODBC driver to do this---however, there may be other options for connecting to Snowflake.

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

During installation make sure to select the correct architecture (32 or 64 Bit)
for the Peakboard runtime, and keep the driver updated because Snowflake
regularly publishes security patches and performance improvements.

In our sample we will use a very simple table that we manually created in the
Snowflake backend. The table is used to store temperature values from an air
conditioning system along with a timestamp and a boolean value to indicate if
the A/C is running at the moment. It's inspired by the OPC UA article we
published 2 years ago about [how to connect to an A/C via OPC
UA](https://how-to-dismantle-a-peakboard-box.com/OPC-UA-Basics-Calling-functions-in-OPC-UA-and-switch-the-AC-off.html).
For production workloads you would typically create the table through an
automated deployment script, apply proper clustering keys, and enable time
travel retention for safer rollbacks and audits.

![image](/assets/2025-11-19/snowflake-temperature-table-sample.png)

## Using ODBC to connect to Snowflake

We will need a connection string to connect to Snowflake. Here's the connection
string related to our Dismantle demo DB. It can be used as template to connect
to the reader's own Snowflake database. To learn about the details of each
parameter we can refer to the
[documentation](https://docs.snowflake.com/en/developer-guide/odbc/odbc-parameters).
The same document also explains how to add optional settings like
`Authenticator=externalbrowser` or `Tracing=6` for troubleshooting, so keep it
at hand while configuring the driver and documenting your setup.

{% highlight text %}
Driver=SnowflakeDSIIDriver; Server=VAAHZZD-XF03787.snowflakecomputing.com; UID=zuteilungsreif; PWD=supersecret; Warehouse=COMPUTE_WH; Database=DISMANTLEDB; Schema=PUBLIC; Role=SYSADMIN;
{% endhighlight %}

Building the connection string was the hardest part. After we solved that we can
use it right away through the ODBC data source in Peakboard designer. The SQL
statement we use is a common statement without any voodoo. A typical source of
error is using the correct namespace to indicate the tables to access. We
already provided a default schema in the connection, so actually we could skip
the fully qualified name within the SQL statement as long as we don't want to
read "outside" the default namespace. So our SQL statement is just `select *
from DISMANTLEDB.PUBLIC.ACLOG` or `select * from ACLOG`. If you need to join
other schemas, simply prefix them with the database name or adjust the default
schema in the connection string, and double-check that the assigned Snowflake
role has the necessary privileges.

![image](/assets/2025-11-19/snowflake-odbc-connection-setup.png)

## Writing back to Snowflake

Uploading huge amounts of data very efficiently is one of the USPs of any
Snowflake database. This is usually done through CSV files. These are uploaded
to a storage (e.g. an Azure storage blob) and then taken from there to upload it
to the database. This process is usually triggered through JSON REST calls (see
documentation above for details). In our sample we will go a different way. It's
less efficient but easy to use and very reliable if the amount of data to insert
is not endlessly huge. We just use an SQL INSERT statement and rely on
Snowflake's automatic transaction handling to keep the data consistent, which
keeps the example approachable for smaller teams.

{% highlight sql %}
INSERT INTO DISMANTLEDB.PUBLIC.ACLOG (TS, Temperature, Cooling)
VALUES    ('2025-07-03 15:15:01', 27.5, False);
{% endhighlight %}

To make it easier to read within Peakboard we use placeholders to build the SQL
string:

{% highlight sql %}
INSERT INTO DISMANTLEDB.PUBLIC.ACLOG (TS, Temperature, Cooling)
VALUES    ('#[TS]#', #[Temperature]#, #[Cooling]#);
{% endhighlight %}

The screenshot shows how to apply the statement and use a placeholder Building
Block to fill the dynamic parts of the statements with real values. You can
further enhance the workflow by adding pre-validation logic to check for
duplicate timestamps or by wrapping the execution in a simple retry block to
cope with transient connectivity issues, especially when the Peakboard box is
deployed in networks with fluctuating bandwidth.

![image](/assets/2025-11-19/snowflake-sql-insert-building-block.png)

## Result and conclusion

This short article shows how easy it is to use ODBC to access Snowflake for both
reading and writing. As long as the throughput is acceptable it's much easier to
use SQL instead of the traditional way of uploading through a file based
interface. Remember to secure the credentials in the connection string, monitor
warehouse usage to avoid unexpected costs, and leverage Snowflake roles to
restrict access to just the tables required by the Peakboard dashboard.

