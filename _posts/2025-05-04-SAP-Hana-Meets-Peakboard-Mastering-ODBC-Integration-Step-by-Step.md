---
layout: post
title: SAP Hana Meets Peakboard - Mastering ODBC Integration Step-by-Step
date: 2023-03-01 03:00:00 +0200
tags: sap bi
image: /assets/2025-05-04/title.png
image_landscape: /assets/2025-05-04/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Download SAP HANA client tools
    url: https://tools.hana.ondemand.com/#hanatools
downloads:
  - name: SAPHanaTest.pbmx
    url: /assets/2025-05-04/SAPHanaTest.pbmx
  - name: warehousetable.txt
    url: /assets/2025-05-04/warehousetable.txt
---
In 2011, SAP launched SAP Hana, a completely new type of database. It was designed to replace SQL Server, Oracle, and Informix as the database backend of choice for SAP systems. Now, more than a decade later, running SAP with anything other than Hana is almost unthinkable.

But Hana can be also used "directly," as normal database, outside the context of SAP. In this article, we will explain how to read from and write to SAP Hana databases, from a Peakboard application.

## Get the endpoint for our SAP Hana database

For this article, we will use a Hana instance hosted on SAP BTP---a cloud service from SAP. For Hana instances that are hosted elsewhere (e.g. AWS, Azure, on-prem), the steps are identical. The only thing that changes is the SQL endpoint.

The following screenshot shows our Hana instance, as it appears in our Hana administration dashboard. To [get the SQL endpoint](https://help.sap.com/docs/hana-cloud-data-lake/interactive-sql-dbisql-context-sensitive-help-for-data-lake-relational-engine/get-sap-hana-database-connection-properties) for our database, we perform these steps:
1. Open the context menu for our endpoint.
2. Select **Copy SQL Endpoint**.

![image](/assets/2025-05-04/010.png)

## Set up client tools

We will use the Hana ODBC driver to access the database. To install it, we follow these steps:

1. Download the [64-bit binaries for Windows](https://tools.hana.ondemand.com/#hanatools). 
2. Unzip the file.
3. Launch `dbinst.exe` to install all necessary client libraries, which includes the ODBC driver.

![image](/assets/2025-05-04/010.png)

## Add an ODBC data source

To access the Hana ODBC driver in the Peakboard Designer, we create a new ODBC data source. It takes two strings:
* "Connection string", which tells the data source how to connect to our database.
* "Statement," which is the SQL statement to executed against the database.

### Connection string
Here are the parts of the connection string:

- `DRIVERS`, the technical name of the ODBC driver.
- `SERVERNODE`, the SQL endpoint which we copied earlier.
- `UID`, the username for the database.
- `PWD`, the password for the database.

Here's an example of a connection string:

{% highlight text %}
DRIVER={HDBODBC};SERVERNODE=f0dfd2cf-d606-4bef-b9ac-8d7baebccb11.hana.trial-us10.hanacloud.ondemand.com:443;UID=DBADMIN;PWD=MyDirtySecret;
{% endhighlight %}

### SQL statement

For our example, we will access a demo table that contains some stock information. If you are following along and want to reproduce the demo table, you can use these [SQL statements](/assets/2025-05-04/warehousetable.txt).

To get the data from the table, we use this SQL statement:
{% highlight sql %}
`SELECT * FROM stock_information`
{% endhighlight %}

![image](/assets/2025-05-04/030.png)

## Install the ODBC driver on the Peakboard Box

To make our app work on a Peakboard Box, we install the ODBC driver on the Box:

1. Copy the installation binaries to the share folder, `\\{Boxname}\Share` (admin credentials needed).
2. Unzip the binaries.
3. Start a PowerShell session and connect to the Box, as described in [this article](/PowerShell-and-Remote-Desktop-How-to-really-dismantle-a-Peakboard-box.html).
4. Execute `\\{Boxname}\Share\dbinst.exe` inside the PowerShell session.

## Result and conclusion

It's straightforward to use ODBC to access databases that are not natively supported by Peakboard. Sometimes, it might be a little tricky to get the correct connection string. It is very important to also install the ODBC driver on the Box. Otherwise, your Peakboard app will not load properly.

This screenshot shows our demo application, running directly on a Box:

![image](/assets/2025-05-04/040.png)