---
layout: post
title: SAP Hana Meets Peakboard: Mastering ODBC Integration Step-by-Step
date: 2023-03-01 03:00:00 +0200
tags: sap bi
image: /assets/2025-05-04/title.png
image_landscape: /assets/2025-05-04/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Download SAP HANA client tools
    url: https://tools.hana.ondemand.com/#hanatools
  - name: YouTube - Getting Started With M Language in Power Query | Basic to Advanced
    url: https://www.youtube.com/watch?v=N8qYRSqRz84&ab_channel=DhruvinShah
  - name: Cracking the code - Part II - Calling functions remotely
    url: /Cracking-the-code-Part-II-Calling-functions-remotely.html
downloads:
  - name: SAPHanaTest.pbix
    url: /assets/2025-05-04/SAPHanaTest.pbmx
  - name: warehousetable.txt
    url: /assets/2025-05-04/warehousetable.txt
---
Back in 2011 SAP launched SAP Hana. A completely new kind of database built to replace SQL Server, Oracle and Informix as typical database backends for SAP systems. Now, more than a decade later, running SAP on other databases than Hana is alomost unthinkable. Beside using Hana as backend for regular SAP applications the database can be also used "directly" as normal database, not enecssarily in the context of tradional SAP business applications.

In today's article we will learn how to use direct access to SAP Hana databases from within the Peakboard application for both reading and writing.

## Get the endpoint to the SAP Hana database

In our example we will use a Hana instance hosted directly at SAP BTP, a cloud based service provided by SAP. Accessing Hana instances that are hosted elesewhere work similiar, only the structure of the SQL endpoint might differ, depending if it's hosted on AWS, Azure or even on prem.

The screenshot shows the running Hana instance in the central cloud administration. In the context menu we have the option to copy the SQL endpoint to access the system. We will need it later. 

![image](/assets/2025-05-04/010.png)

## Set up the client tools

We will use the Hana ODBC driver to access the system. It can be download from the [Hana tools website](https://tools.hana.ondemand.com/#hanatools). We will need the 64 bit binaries for windows. After unzipping the file we can launch the executable dbinst.exe to install all necessary client libraries, which includes the ODBC driver.

![image](/assets/2025-05-04/010.png)

## Using the ODBC data source

To access the Hana ODBC driver in the Peakboard designer we create a new data ODBC data source. It only has one text field for the connection string and one for the SQL command to be executed against the database.
Here are the parts of the connection string:

- DRIVERS is the techical name of the ODBC driver
- SERVERNODE is the SQL endpoint which we copied earlier
- UID, PWD are the values for user name and password to access the database

Here's a sample of how a connection string should look like:

{% highlight text %}
DRIVER={HDBODBC};SERVERNODE=f0dfd2cf-d606-4bef-b9ac-8d7baebccb11.hana.trial-us10.hanacloud.ondemand.com:443;UID=DBADMIN;PWD=MyDirtySecret;
{% endhighlight %}

In our example we will access a demo table that contains some stock information. To rebuild the table we can use [this SQL statement](/assets/2025-05-04/warehousetable.txt). So the corresponding SQL statement is "select * from stock_information" to retrieve the data.

![image](/assets/2025-05-04/030.png)

## Get the ODBC driver on the box

To make the pbmx work on a Peakboard box, we need to make the ODBC driver available on the box. Here's the step-by-step guide on how to do that:

1. Copy the installation binaries to the share folder \\{Boxname}\Share (we will need admin credentials to do this)
2. Unzip it
3. Open a Powershell session to the box as described in [this article](/PowerShell-and-Remote-Desktop-How-to-really-dismantle-a-Peakboard-box.html)
4. Execute \\{Boxname}\Share\dbinst.exe within the Powershell session

## result and conclusion

Using ODBC to access databases that are not natively supported by Peakboard is straight forward. Sometimes it might by tricky to build the right connection string. It is very important to install the ODBC driver binaries also on the box, otherwise the Peakboard app will fail to load properly.
The final screenshot shows the demo pbmx of our example as it runs directly on the box.

![image](/assets/2025-05-04/040.png)